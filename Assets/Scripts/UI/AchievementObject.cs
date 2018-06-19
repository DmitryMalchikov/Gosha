using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementObject : MonoBehaviour {

    public Text Title;

    public PlayerAchievementModel info;

    public void SetAchievement(PlayerAchievementModel model)
    {
        info = model;
        Title.text = model.Name;
    }

    public void OpenAchievementInfo()
    {
        Canvaser.Instance.AchievementInfo.SetAchievementInfo(info);
    }
}
