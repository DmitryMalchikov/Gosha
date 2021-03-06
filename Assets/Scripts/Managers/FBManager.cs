﻿using UnityEngine;
using Facebook.Unity;

public class FBManager : Singleton<FBManager>
{
    private string _currentAchievement;

    private void Start()
    {
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
			AchievementsManager.Instance.CheckAchievements(TasksTypes.Share);
        }
    }
}
