using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : MonoBehaviour
{
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
        SetStatisticsPanel(leaders, AllTimeLeaders, AllTimeLeadersContent);
    }

    public void SetLeaders(List<FriendOfferStatisticsModel> leaders)
    {
        SetStatisticsPanel(leaders, Leaders, LeadersContent);
    }

    public void SetStatisticsPanel(List<FriendOfferStatisticsModel> leaders, List<FriendObject> friendList, Transform parentPanel)
    {
        for (int i = 0; i < leaders.Count; i++)
        {
            FriendObject newLeader = Instantiate(Leader, parentPanel).GetComponent<FriendObject>();
            if (leaders[i].Id == LoginManager.User.Id)
            {
                newLeader.YourPanelTournament(leaders[i], i);
            }
            else
            {
                newLeader.SetStatisticsObject(leaders[i], i);
            }
            friendList.Add(newLeader);
        }
    }

    public void ClearContent(Transform content, List<FriendObject> list)
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
