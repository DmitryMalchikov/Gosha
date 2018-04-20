using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeeklyTask : MonoBehaviour {

    protected WaitForSeconds minute = new WaitForSeconds(60);

    public Text Task;
    public Text TimeLeft;
    PlayerTaskModel info;

    public GameObject IsLocked;

    public void SetTask(int number, PlayerTaskModel model)
    {
        info = model;
        Task.text = model.GenerateDescription();

        if(info.PlayerProgress >= info.ActionsCount)
        {
            IsLocked.SetActive(false);
        }        
    }

	private void OnEnable(){
		StartCoroutine(CheckTime());
	}

    protected IEnumerator CheckTime()
    {
		yield return new WaitUntil (() => info != null);

        while (true)
        {
            var time = (info.ExpireDate - DateTime.UtcNow);

            TimeLeft.text = time.IntervalToString();

            yield return minute;
        }
    }
}
