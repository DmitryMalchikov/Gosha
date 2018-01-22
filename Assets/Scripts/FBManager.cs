using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class FBManager : MonoBehaviour
{
    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init();
        }
        else
        {
            FB.ActivateApp();
        }
    }

    public void LogIn()
    {
        FB.LogInWithReadPermissions(callback: OnLogIn);
    }

    private void OnLogIn(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Share();
        }
    }

    public void OpenShare()
    {
        if (FB.IsLoggedIn)
        {
            Share();
        }
        else
        {
            LogIn();
        }
    }

    public void Share()
    {
        FB.ShareLink(contentTitle: "Gosha game", contentDescription: "i earned points!", callback: OnShare);
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
