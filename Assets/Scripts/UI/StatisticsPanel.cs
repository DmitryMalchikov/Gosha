using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : MonoBehaviour {

    public GameObject Leader;
    public Transform AllTimeLeadersContent;
    public Transform LeadersContent;
    public ScrollRect Scroll;

    public List<FriendObject> AllTimeLeaders;
    public List<FriendObject> Leaders;

    public Text LeadersText;
    public Toggle WeekToggle;
    public Toggle MonthToggle;

    public void Open()
    {
        gameObject.SetActive(true);
        ClearAllContent();

        MonthToggle.isOn = false;
        WeekToggle.isOn = true;
        StatisticsManager.Instance.GetAllStatisticsAsync(0, () => Scroll.Rebuild(CanvasUpdate.PostLayout));
        StatisticsManager.Instance.GetAllStatisticsAsync(2, () => Scroll.Rebuild(CanvasUpdate.PostLayout));
    }

    public void UpdateInfo(int val)
    {
        ClearContent(LeadersContent, Leaders);
        StatisticsManager.Instance.GetAllStatisticsAsync(val, () => Scroll.Rebuild(CanvasUpdate.PostLayout));
    }

    private void ClearAllContent()
    {
        ClearContent(LeadersContent, Leaders);
        ClearContent(AllTimeLeadersContent, AllTimeLeaders);
    }

    public void SetAllTimeLeaders(List<FriendOfferStatisticsModel> leaders)
    {
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
