using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DTO;
using Assets.Scripts.Gameplay;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Managers
{
    public class AchievementsManager : APIManager<AchievementsManager>
    {
        public List<PlayerTasks> Achievements = new List<PlayerTasks>();

        public string GetAchievementsUrl = "/api/account/achievementsinfo";
        public string SubmitAchievementUrl = "/api/achievements/submitachievement";

        public override void SetUrls()
        {
            ServerInfo.SetUrl(ref GetAchievementsUrl);
            ServerInfo.SetUrl(ref SubmitAchievementUrl);
        }

        public void LoadAchievements(PlayerTasks[] achievements)
        {
            for (int i = 0; i < achievements.Length; i++)
            {
                achievements[i].PlayerStartProgress = achievements[i].PlayerProgress;
                Achievements.Add(achievements[i]);
            }
        }

        public void CheckAchievements(TasksTypes type, int completeAmount = 1)
        {
            List<PlayerTasks> achievements = Achievements.Where(ach => ach.Type == type.ToString()).ToList();
            bool send = type == TasksTypes.Buy || type == TasksTypes.Share;

            for (int i = 0; i < achievements.Count; i++)
            {
                achievements[i].PlayerProgress += completeAmount;

                if (achievements[i].PlayerProgress >= achievements[i].ActionsCount || send)
                {
                    SubmitAchievementAsync(achievements[i], type);

                    if (achievements[i].PlayerProgress < achievements[i].ActionsCount) continue;

                    achievements.RemoveAt(i);
                    i--;
                }
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause && GameController.Started)
            {
                SubmitAllAchievements(false);
            }
        }

        public void SubmitAllAchievements(bool reset)
        {
            for (int i = 0; i < Achievements.Count; i++)
            {
                if (!Achievements[i].InOneRun)
                {
                    SubmitAchievementAsync(Achievements[i], TasksTypes.All);
                }
                else if (reset)
                {
                    Achievements[i].PlayerProgress = 0;
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
}
