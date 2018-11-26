using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentPanel : TimeCheck
{
    public Text TournamentText;
    public Text TournamentName;
    public Text Region;
    public Text Goal;
    public Text FirstPlacePrize;
    public Text SecondPlacePrize;
    public Text ThirdPlacePrize;
    public Transform StatisticsContent;
    public GameObject TournamentPlayer;
    public ScrollRect ContentScroll;
    public GameObject GoalPanel;

    public List<FriendObject> Participants;

    private TournamentModel _info;

    public override IExpirable Info
    {
        get
        {
            return _info;
        }
    }

    public void SetTournamentInfo(TournamentModel model)
    {
        if (!string.IsNullOrEmpty(model.Name))
        {
            Canvaser.Instance.TournamentBtn.interactable = true;
            _info = model;
            TournamentName.text = model.Name;
            Region.text = LocalizationManager.GetLocalizedValue("region") + LocalizationManager.GetLocalizedValue(LoginManager.User.Region);
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

    public void SetTournamentTable(FriendOfferStatisticsModel[] models)
    {
        ClearContent();

        for (int i = 0; i < models.Length; i++)
        {
            FriendObject newParticipant = Instantiate(TournamentPlayer, StatisticsContent).GetComponent<FriendObject>();
            if (models[i].Id == LoginManager.User.Id)
            {
                newParticipant.YourPanelTournament(models[i], i);
            }
            else
            {
                newParticipant.SetTournamentObject(models[i], i);
            }

            Participants.Add(newParticipant);
        }
    }

    public void OpenTable()
    {
        gameObject.SetActive(true);
        StartCoroutine(EnableGoal());
    }

    IEnumerator EnableGoal()
    {
        yield return null;
        GoalPanel.SetActive(true);
    }

    public void ClearContent()
    {
        StatisticsContent.ClearContent();
        Participants.Clear();
    }

    public void OpenTournament()
    {
        gameObject.SetActive(true);
        StartCoroutine(CheckTime());
    }
}
