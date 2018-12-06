using Assets.Scripts.Utils;

namespace Assets.Scripts.Managers
{
    public class VKManager : Singleton<VKManager>
    {
        private string _currentAchievement;

        public void OpenShare(string achievementName)
        {
            _currentAchievement = achievementName;

            if (VK.IsLoggedIn())
            {
                WallPost();
            }
            else
            {
                LogIn();
            }
        }

        private void LogIn()
        {
            VK.Login(OnLogIn);
        }

        private void WallPost()
        {
            VK.WallPost("Я играю в GoGo Gosha, а ты?\n" + _currentAchievement, "http://gosha.by/Html/HomePage.html", "Go-go Gosha!", LoginManager.Instance.ShareImageUrl
                , (response) =>
                {
                    if (string.IsNullOrEmpty(response))
                    {
                        AchievementsManager.Instance.CheckAchievements(TasksTypes.Share);
                    }
                    else
                    {

                    }
                });
        }

        private void OnLogIn(string result)
        {
            if (result == "success")
            {
                WallPost();
            }
        }
    }
}
