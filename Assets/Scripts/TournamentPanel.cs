using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class TournamentPanel : MonoBehaviour {

    public Text TournamentText;
    public Text TournamentName;
    public Text Region;

    public Text Goal;
    public Text TimeToEnd;
    public Text FirstPlacePrize;
    public Text SecondPlacePrize;
    public Text ThirdPlacePrize;

    public Transform StatisticsContent;
    public GameObject TournamentPlayer;

    public List<FriendObject> Participants;

    public TournamentModel info;

    protected WaitForSeconds minute = new WaitForSeconds(60);

    public void SetTournamentInfo(TournamentModel model)
    {
        if (!string.IsNullOrEmpty(model.Name))
        {
            Canvaser.Instance.TournamentBtn.interactable = true;
            info = model;
            TournamentName.text = model.Name;
            Region.text = LocalizationManager.GetLocalizedValue("region") + LocalizationManager.GetLocalizedValue(LoginManager.Instance.User.Region);
            string[] prizes = new string[3];
            Debug.Log(model.Prizes);
            prizes = (model.Prizes.Split(';'));
            FirstPlacePrize.text = LocalizationManager.GetLocalizedValue("1stplace") + prizes[0];
            SecondPlacePrize.text = LocalizationManager.GetLocalizedValue("2ndplace") + prizes[1];
            ThirdPlacePrize.text = LocalizationManager.GetLocalizedValue("3rdplace") + prizes[2];
        }
        else
        {
            Canvaser.Instance.TournamentBtn.interactable = false;
        }

		Canvaser.Instance.WeeklyTasksBtn.interactable = model.AvaliableWeeklyTasks;
    }

    public void SetTournamentTable(List<FriendModel> models)
    {
        ClearContent();

        for (int i = 0; i < models.Count; i++)
        {
            FriendObject newParticipant = Instantiate(TournamentPlayer, StatisticsContent).GetComponent<FriendObject>();
            newParticipant.SetTournamentObject(models[i],i);
            Participants.Add(newParticipant);
        }
        gameObject.SetActive(true);
        StartCoroutine(CheckTime());
    }

    public void ClearContent()
    {
        foreach (Transform item in StatisticsContent)
        {
            Destroy(item.gameObject);
        }
        Participants.Clear();
    }

    protected IEnumerator CheckTime()
    {
        while (true)
        {
            var time = (info.ExpireDate - DateTime.Now);

			StringBuilder timeLeft = new StringBuilder ();

			if (time.Days > 0) {
				timeLeft.AppendFormat("{0:00} {1} ", time.Days, LocalizationManager.GetLocalizedValue("days"));
			}
			if (time.Hours > 0) {
				timeLeft.AppendFormat("{0:00} {1} ", time.Hours, LocalizationManager.GetLocalizedValue("hours"));
			}

			timeLeft.AppendFormat("{0:00} {1}", time.Minutes, LocalizationManager.GetLocalizedValue("minutes"));

			TimeToEnd.text = timeLeft.ToString();

            yield return minute;
        }
    }

    public void OpenTournament()
    {
        gameObject.SetActive(true);
        StartCoroutine(CheckTime());
    }
}
