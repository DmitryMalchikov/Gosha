using Odnoklassniki;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class OKManager : Singleton<OKManager>
{
    private string _currentAchievement;

    private void Start()
    {
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
        else if (OK.AccessTokenExpiresAt < DateTime.Now)
        {
            OK.RefreshOAuth(success =>
                {
                    StartCoroutine(Share("Я играю в GoGo Gosha, а ты?\n" + _currentAchievement, "http://gosha.by/Html/HomePage.html", "Go-go Gosha!", LoginManager.Instance.ShareImageUrl));
                });
        }
        else
        {
            StartCoroutine(Share("Я играю в GoGo Gosha, а ты?\n" + _currentAchievement, "http://gosha.by/Html/HomePage.html", "Go-go Gosha!", LoginManager.Instance.ShareImageUrl));
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
        if (success)
        {
            StartCoroutine(Share("Я играю в GoGo Gosha, а ты?\n" + _currentAchievement, "http://gosha.by/Html/HomePage.html", "Go-go Gosha!", LoginManager.Instance.ShareImageUrl));
        }
    }

    private void Share(string text, string link, string title, Texture2D image)
    {
        OK.OpenPublishDialog(
            response =>
            {
                if (response != null && response.Object != null && response.Object.ContainsKey("error_code"))
                {
                }
                else if (response != null)
                {
                    AchievementsManager.Instance.CheckAchievements(TasksTypes.Share);
                }
            },
            new List<OKMedia>()
            {
                OKMedia.Text(text),
                OKMedia.Photo(image),
                OKMedia.Link(link)
            });
    }

    IEnumerator Share(string text, string link, string title, string imageLink)
    {
        WWW image = new WWW(imageLink);
        yield return image;

        if (image.texture)
        {
            Share(text, link, title, image.texture);
        }
    }
}
