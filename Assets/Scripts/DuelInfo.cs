using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DuelInfo : MonoBehaviour {

    protected WaitForSeconds minute = new WaitForSeconds(60);

    public Text Name;
    public Text Bet;
    public Text Time;
    public Image Avatar;

    public DuelModel info;

    public virtual void SetDuelPanel(DuelModel model)
    {
        info = model;
        Name.text = info.Nickname;
        Bet.text = info.Bet.ToString();
        LoginManager.Instance.GetUserImage(info.UserId, Avatar);        
    }

	void OnEnable(){
		StartCoroutine (CheckTime ());
	}

    public void AcceptDuel(bool accept)
    {
        if (accept)
        {
            DuelManager.Instance.AcceptDuelAsync(info.Id);
            Run();
        }
        else
        {
            DuelManager.Instance.DeclineDuelAsync(info.Id);
        }
    }

    public void Run()
    {
        Canvaser.Instance.Duels.gameObject.SetActive(false);
        GameController.Instance.InDuel = true;
        GameController.Instance.DuelID = info.Id;
        Canvaser.Instance.StartRun();
    }

    protected IEnumerator CheckTime()
    {
		yield return new WaitUntil (() => info != null);

        while (true)
        {
            var time = (info.ExpireDate - DateTime.Now.ToUniversalTime());

            Time.text = string.Format("{0:00}ч {1:00}мин", time.Hours, time.Minutes);

            yield return minute;
        }
    }
}
