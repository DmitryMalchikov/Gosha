using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VKManager : MonoBehaviour
{
    

    public void OpenShare()
    {
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
        VK.WallPost("sas", "https://vk.com/gordiri", "sas", (response) =>
        {
            if (string.IsNullOrEmpty(response))
            {
					AchievementsManager.Instance.CheckAchievements(TasksTypes.ShareVK);
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
