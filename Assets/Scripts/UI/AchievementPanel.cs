using System.Collections.Generic;
using UnityEngine;

public class AchievementPanel : MonoBehaviour {

    public Transform DoneAchievementsContent;
    public Transform AchievementsContent;

    public GameObject AchievementObj;
    public List<AchievementObject> Achievements;

    public void Open()
    {
        if (Canvaser.Instance.IsLoggedIn())
        {
            Canvaser.Instance.AchievementsPanel.gameObject.SetActive(true);
            AchievementsManager.Instance.GetAllAchievementsAsync();
        }
    }

    public void SetAchievementsPanel(List<PlayerAchievementModel> models)
    {
        ClearContent();
        foreach (PlayerAchievementModel item in models)
        {
            AchievementObject newAchievement;
            if (item.PlayerProgress >= item.ActionsCount)
            {
                newAchievement = Instantiate(AchievementObj, DoneAchievementsContent).GetComponent<AchievementObject>();
            }
            else
            {
                newAchievement = Instantiate(AchievementObj, AchievementsContent).GetComponent<AchievementObject>();
            }
            newAchievement.SetAchievement(item);
            Achievements.Add(newAchievement);
        }
        gameObject.SetActive(true);
    }

    public void ClearContent()
    {
        AchievementsContent.ClearContent();
        DoneAchievementsContent.ClearContent();
        Achievements.Clear();
    }
}
