using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentsController : MonoBehaviour {
    public List<PlayerAchievementModel> RunAchievements = new List<PlayerAchievementModel>();
    public List<PlayerAchievementModel> JumpAchievements = new List<PlayerAchievementModel>();
    public List<PlayerAchievementModel> PlayAchievements = new List<PlayerAchievementModel>();
    public List<PlayerAchievementModel> CollectBonusAchievements = new List<PlayerAchievementModel>();
    public List<PlayerAchievementModel> CollectIceCreamAchievements = new List<PlayerAchievementModel>();
    public List<PlayerAchievementModel> BuyAchievements = new List<PlayerAchievementModel>();
    public List<PlayerAchievementModel> LooseAchievements = new List<PlayerAchievementModel>();
    public List<PlayerAchievementModel> ShareAchievements = new List<PlayerAchievementModel>();

    public void LoadAchievements(List<PlayerAchievementModel> achievements)
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            switch (achievements[i].Type)
            {
                case "Run":
                    RunAchievements.Add(achievements[i]);
                    break;
                case "Jump":
                    JumpAchievements.Add(achievements[i]);
                    break;
                case "Play":
                    PlayAchievements.Add(achievements[i]);
                    break;
                case "CollectBonus":
                    CollectBonusAchievements.Add(achievements[i]);
                    break;
                case "CollectIceCream":
                    CollectIceCreamAchievements.Add(achievements[i]);
                    break;
                case "Buy":
                    BuyAchievements.Add(achievements[i]);
                    break;
                case "Loose":
                    LooseAchievements.Add(achievements[i]);
                    break;
                case "Share":
                    ShareAchievements.Add(achievements[i]);
                    break;
            }
        }
    }

    public void CheckAchievements(string type, int completeAmount)
    {
        List<PlayerAchievementModel> achievements = new List<PlayerAchievementModel>();

        switch (type)
        {
            case "Run":
                achievements = RunAchievements;
                break;
            case "Jump":
                achievements = JumpAchievements;
                break;
            case "Play":
                achievements = PlayAchievements;
                break;
            case "CollectBonus":
                achievements = CollectBonusAchievements;
                break;
            case "CollectIceCream":
                achievements = CollectIceCreamAchievements;
                break;
            case "Buy":
                achievements = BuyAchievements;
                break;
            case "Loose":
                achievements = LooseAchievements;
                break;
            case "Share":
                achievements = ShareAchievements;
                break;
        }

        for (int i = 0; i < achievements.Count; i++)
        {
            achievements[i].PlayerProgress += completeAmount;

            if (achievements[i].PlayerProgress > achievements[i].ActionsCount)
            {
                //complete achievement
                //send to server then show pop up
                achievements.RemoveAt(i);
                i--;
            }
        }
    }

}
