using Odnoklassniki;
using System.Collections.Generic;
using UnityEngine;

public class OKManager : MonoBehaviour
{
    public static OKManager Instance;
    private string _currentAchievement;

    private void Awake()
    {
        Instance = this;

        if (!OK.IsInitialized)
        {
            OK.Init();
        }
    }

    public void OpenShare(string achievementName)
    {
        _currentAchievement = achievementName;

        if (!OK.IsLoggedIn)
        {
            LogIn();
        }
        else
        {
            Share("Я играю в GoGo Gosha, а ты?\n" + _currentAchievement, "Go-go Gosha!", Resources.Load<Texture2D>("ShareScreen"));
        }
    }

    private void LogIn()
    {
        if (!OK.IsLoggedIn)
        {
            OK.Auth(OnLogIn);
        }
    }

    private void OnLogIn(bool success)
    {
        Share("Я играю в GoGo Gosha, а ты?\n" + _currentAchievement, "Go-go Gosha!", Resources.Load<Texture2D>("ShareScreen"));
    }

    private void Share(string text, string title, Texture2D image)
    {
        OK.OpenPublishDialog(
            response =>
            {
                if (response.Object != null && response.Object.ContainsKey("error_code"))
                {

                }
                else
                {
					AchievementsManager.Instance.CheckAchievements(TasksTypes.ShareOK);
                }
            },
            new List<OKMedia>()
            {
                OKMedia.Text(text),
                OKMedia.Photo(image)
            });
    }
}
