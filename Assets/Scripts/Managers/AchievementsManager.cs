﻿using System.Collections.Generic;

public class AchievementsManager : APIManager<AchievementsManager>
{
    public List<PlayerTasks> RunAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> JumpAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> CollectIceCreamAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> BuyAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> LooseAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> ShareAchievements = new List<PlayerTasks>();

    public string GetAchievementsUrl = "/api/account/achievementsinfo";
    public string SubmitAchievementUrl = "/api/achievements/submitachievement";

    public override void SetUrls()
    {
        ServerInfo.SetUrl(ref GetAchievementsUrl);
        ServerInfo.SetUrl(ref SubmitAchievementUrl);
    }

    public void LoadAchievements(List<PlayerTasks> achievements)
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            achievements[i].PlayerStartProgress = achievements[i].PlayerProgress;

            switch (achievements[i].Type)
            {
                case "Run":
                    RunAchievements.Add(achievements[i]);
                    break;
                case "Jump":
                    JumpAchievements.Add(achievements[i]);
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

    public void CheckAchievements(TasksTypes type, int completeAmount = 1)
    {
        List<PlayerTasks> achievements = new List<PlayerTasks>();
        bool send = false;

        switch (type)
        {
            case TasksTypes.Run:
                achievements = RunAchievements;
                break;
            case TasksTypes.Jump:
                achievements = JumpAchievements;
                break;
            case TasksTypes.CollectIceCream:
                achievements = CollectIceCreamAchievements;
                break;
            case TasksTypes.Buy:
                achievements = BuyAchievements;
                send = true;
                break;
            case TasksTypes.Loose:
                achievements = LooseAchievements;
                break;
            case TasksTypes.Share:
                achievements = ShareAchievements;
                send = true;
                break;
        }

        for (int i = 0; i < achievements.Count; i++)
        {
            achievements[i].PlayerProgress += completeAmount;

            if (achievements[i].PlayerProgress >= achievements[i].ActionsCount || send)
            {
                SubmitAchievementAsync(achievements[i], type);

                if (achievements[i].PlayerProgress >= achievements[i].ActionsCount)
                {
                    achievements.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && GameController.Instance.Started)
        {
            SubmitAllAchievements(false);
        }
    }

    public void SubmitAllAchievements(bool reset)
    {
        List<PlayerTasks> achievements = new List<PlayerTasks>();

        achievements.AddRange(RunAchievements);
        achievements.AddRange(JumpAchievements);
        achievements.AddRange(CollectIceCreamAchievements);
        achievements.AddRange(BuyAchievements);
        achievements.AddRange(LooseAchievements);
        achievements.AddRange(ShareAchievements);

        for (int i = 0; i < achievements.Count; i++)
        {
            if (!achievements[i].InOneRun)
            {
                //complete achievement
                //send to server then show pop up
                SubmitAchievementAsync(achievements[i], TasksTypes.All);
            }
            else if (reset)
            {
                achievements[i].PlayerProgress = 0;
            }
        }
    }


    void SubmitAchievementAsync(PlayerTasks model, TasksTypes type)
    {
        SubmitAchievementModel value = new SubmitAchievementModel() { Id = model.Id, AchievementId = model.TaskId, PlayerProgress = model.PlayerProgress - model.PlayerStartProgress, Language = (int)LocalizationManager.CurrentLanguage };
        CoroutineManager.SendRequest(SubmitAchievementUrl, value, (PlayerTasksAnswer newModel) =>
       {
           model.Id = newModel.Id;
           model.PlayerStartProgress = model.PlayerProgress;

           if (model.PlayerProgress >= model.ActionsCount)
           {
               Canvaser.Instance.PopUpPanel.ShowAchievement(newModel);
           }

           if (type == TasksTypes.Share && Canvaser.Instance.Suits.gameObject.activeInHierarchy)
           {
               InventoryManager.Instance.GetSuitsAsync(true);
           }
            //add window
        });
    }

    public void GetAllAchievementsAsync(ResultCallback callback = null)
    {
        CoroutineManager.SendRequest(GetAchievementsUrl, new { Value = (int)LocalizationManager.CurrentLanguage }, (List<PlayerAchievementModel> tasks) =>
        {
            Canvaser.Instance.AchievementsPanel.SetAchievementsPanel(tasks);

            if (callback != null)
            {
                callback();
            }
        }, loadingPanelsKey: "achievements");
    }
}
