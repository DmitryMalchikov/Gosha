using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{

    public static AchievementsManager Instance { get; private set; }

    public List<PlayerTasks> RunAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> JumpAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> CollectIceCreamAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> BuyAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> LooseAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> ShareVKAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> ShareFBAchievements = new List<PlayerTasks>();
    public List<PlayerTasks> ShareOKAchievements = new List<PlayerTasks>();

    public string GetAchievementsUrl = "/api/account/achievementsinfo";
    public string SubmitAchievementUrl = "/api/achievements/submitachievement";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetUrls();
    }

    public void SetUrls()
    {
        GetAchievementsUrl = ServerInfo.GetUrl(GetAchievementsUrl);
        SubmitAchievementUrl = ServerInfo.GetUrl(SubmitAchievementUrl);
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
                case "ShareVK":
                    ShareVKAchievements.Add(achievements[i]);
                    break;
                case "ShareFB":
                    ShareFBAchievements.Add(achievements[i]);
                    break;
                case "ShareOK":
                    ShareOKAchievements.Add(achievements[i]);
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
		case TasksTypes.ShareVK:
                achievements = ShareVKAchievements;
                send = true;
                break;
		case TasksTypes.ShareFB:
                achievements = ShareFBAchievements;
                send = true;
                break;
		case TasksTypes.ShareOK:
                achievements = ShareOKAchievements;
                send = true;
                break;
        }

        for (int i = 0; i < achievements.Count; i++)
        {
            achievements[i].PlayerProgress += completeAmount;

            if (achievements[i].PlayerProgress >= achievements[i].ActionsCount || send)
            {
                SubmitAchievementAsync(achievements[i]);

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
        achievements.AddRange(ShareVKAchievements);
        achievements.AddRange(ShareFBAchievements);
        achievements.AddRange(ShareOKAchievements);

        for (int i = 0; i < achievements.Count; i++)
        {
            if (!achievements[i].InOneRun)
            {
                //complete achievement
                //send to server then show pop up
                SubmitAchievementAsync(achievements[i]);
            }
            else if (reset)
            {
                achievements[i].PlayerProgress = 0;
            }
        }
    }
    

    void SubmitAchievementAsync(PlayerTasks model)
    {
        SubmitAchievementModel value = new SubmitAchievementModel() { Id = model.Id, AchievementId = model.TaskId, PlayerProgress = model.PlayerProgress - model.PlayerStartProgress, Language = (int)LocalizationManager.CurrentLanguage };
        StartCoroutine(NetworkHelper.SendRequest(SubmitAchievementUrl, value, "application/json", (response) =>
        {
            PlayerTasksAnswer newModel = JsonConvert.DeserializeObject<PlayerTasksAnswer>(response.Text);

            model.Id = newModel.Id;
            model.PlayerStartProgress = model.PlayerProgress;

            if (model.PlayerProgress >= model.ActionsCount)
            {
                Canvaser.Instance.PopUpPanel.ShowAchievement(newModel);
            }
            //add window
        }));
    }

    public void GetAllAchievementsAsync(ResultCallback callback=null)
    {
        StartCoroutine(NetworkHelper.SendRequest(GetAchievementsUrl, new { Value = (int)LocalizationManager.CurrentLanguage}, "application/json", (response) => 
        {
            List<PlayerAchievementModel> tasks = JsonConvert.DeserializeObject<List<PlayerAchievementModel>>(response.Text);
            Canvaser.Instance.AchievementsPanel.SetAchievementsPanel(tasks);

            if (callback != null)
            {
                callback();
            }
        }));
    }
}
