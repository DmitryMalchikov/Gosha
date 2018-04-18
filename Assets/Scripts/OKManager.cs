using Odnoklassniki;
using System.Collections.Generic;
using UnityEngine;

public class OKManager : MonoBehaviour
{   

    private void Awake()
    {
        if (!OK.IsInitialized)
        {
            OK.Init();
        }
    }

    public void OpenShare()
    {
        if (!OK.IsLoggedIn)
        {
            LogIn();
        }
        else
        {
            Share("", "", new Texture2D(20, 20));
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
        Share("", "", new Texture2D(20, 20));
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
