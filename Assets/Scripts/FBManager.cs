using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class FBManager : MonoBehaviour
{
    public static FBManager Instance;
    private string _currentAchievement;

    private void Awake()
    {
        Instance = this;

        if (!FB.IsInitialized)
        {
			FB.Init(OnInitComplete);
        }
        else
        {
            FB.ActivateApp();
        }
    }

	private void OnInitComplete(){
		Debug.Log("Init completed");
	}

    public void LogIn()
    {
        FB.LogInWithReadPermissions(callback: OnLogIn);
    }

    private void OnLogIn(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Share(_currentAchievement);
        }
    }

    public void OpenShare(string achievementName)
    {
        _currentAchievement = achievementName;

        if (FB.IsLoggedIn)
        {
            Share(_currentAchievement);
        }
        else
        {
            LogIn();
        }
    }

    public void Share(string achievementName)
    {
        FB.FeedShare(linkCaption: "Я играю в GoGo Gosha, а ты?\n" + _currentAchievement, 
			link: new System.Uri("http://gosha.by/Html/HomePage.html"), 
			linkName: "Go-go Gosha!",
			callback: OnShare,
            picture: new System.Uri(LoginManager.Instance.ShareImageUrl));       
    }

    private void OnShare(IShareResult result)
    {
        if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {

        }
        else
        {
			AchievementsManager.Instance.CheckAchievements(TasksTypes.ShareFB);
        }
    }
}
