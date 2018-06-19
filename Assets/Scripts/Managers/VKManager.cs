using UnityEngine;

public class VKManager : MonoBehaviour
{
    public static VKManager Instance;
    private string _currentAchievement;

    private void Awake()
    {
        Instance = this;
    }

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
