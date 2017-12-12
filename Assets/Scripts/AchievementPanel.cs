using System.Collections.Generic;
using UnityEngine;

public class AchievementPanel : MonoBehaviour {

    public Transform DoneAchievementsContent;
    public Transform AchievementsContent;

    public GameObject AchievementObj;
    public List<AchievementObject> Achievements;

    public void Open()
    {
        Canvaser.ShowLoading(true);
        AchievementsManager.Instance.GetAllAchievementsAsync(() => Canvaser.ShowLoading(false));
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
        foreach(Transform item in AchievementsContent)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in DoneAchievementsContent)
        {
            Destroy(item.gameObject);
        }
        Achievements.Clear();
    }
}
