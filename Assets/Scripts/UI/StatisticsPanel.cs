using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : MonoBehaviour {

    public GameObject Leader;
    public Transform AllTimeLeadersContent;
    public Transform LeadersContent;

    public List<FriendObject> AllTimeLeaders;
    public List<FriendObject> Leaders;

    public Dropdown dropdown;

    public Text LeadersText;

    public void Open()
    {
        gameObject.SetActive(true);

        StatisticsManager.Instance.GetAllStatisticsAsync(dropdown.value);
        StatisticsManager.Instance.GetAllStatisticsAsync(2);

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            dropdown.options[i].text = LocalizationManager.GetLocalizedValue("statdropdown" + i);
        }
    }

    public void UpdateInfo(int val)
    {
        //switch(val)
        //{
        //    case 0:
        //        LeadersText.text = LocalizationManager.GetLocalizedValue("weekleaders");
        //        break;
        //    case 1:
        //        LeadersText.text = LocalizationManager.GetLocalizedValue("monthleaders");
        //        break;
        //}
        ClearContent(LeadersContent, Leaders);
        StatisticsManager.Instance.GetAllStatisticsAsync(val);
    }

    public void SetAllTimeLeaders(List<FriendOfferStatisticsModel> leaders)
    {
        ClearContent(AllTimeLeadersContent, AllTimeLeaders);
        for (int i = 0; i < leaders.Count; i++)
        {
            FriendObject newLeader = Instantiate(Leader, AllTimeLeadersContent).GetComponent<FriendObject>();
            if (leaders[i].Id == LoginManager.Instance.User.Id)
            {
                newLeader.YourPanelTournament(leaders[i], i);
            }
            else
            {
                newLeader.SetTournamentObject(leaders[i], i);
            }
            AllTimeLeaders.Add(newLeader);
        }
        
    }

    public void SetLeaders(List<FriendOfferStatisticsModel> leaders)
    {
        ClearContent(LeadersContent, Leaders);
        for (int i = 0; i < leaders.Count; i++)
        {
            FriendObject newLeader = Instantiate(Leader, LeadersContent).GetComponent<FriendObject>();
            if (leaders[i].Id == LoginManager.Instance.User.Id)
            {
                newLeader.YourPanelTournament(leaders[i], i);
            }
            else
            {
                newLeader.SetTournamentObject(leaders[i], i);
            }
            Leaders.Add(newLeader);
        }
    }

    public void ClearContent(Transform content,List<FriendObject> list)
    {
        bool loading = false;
        foreach (Transform item in content)
        {
            if (loading)
            {
                Destroy(item.gameObject);
            }
            else
            {
                loading = true;
            }
        }
        list.Clear();
    }
}
