using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementInfo : MonoBehaviour {

    public PlayerAchievementModel info;

    public Text Title;
    public Text Description;
    public GameObject SocialNetworkButtons;

    public Image ADS;
    public Text ADSText;

    public void SetAchievementInfo(PlayerAchievementModel model)
    {
        info = model;
        Title.text = model.Name;
        Description.text = model.GenerateDescription();
        SocialNetworkButtons.SetActive(model.PlayerProgress >= model.ActionsCount);
        AdsManager.Instance.GetAds(ADSText, ADS);
        gameObject.SetActive(true);
    }

    public void ShareFB()
    {

    }

    public void ShareVK()
    {

    }

    public void ShareOK()
    {

    }
}
