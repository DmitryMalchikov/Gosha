using UnityEngine;
using UnityEngine.UI;

public class AchievementInfo : MonoBehaviour
{
    public PlayerAchievementModel info;

    public Text Title;
    public Text Description;
    public GameObject SocialNetworkButtons;

    public RawImage ADS;
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

    public void SetAchievementInfo()
    {
        Title.text = LocalizationManager.GetLocalizedValue("share");
        Description.text = LocalizationManager.GetLocalizedValue("sharegamewithyourfriends");
        SocialNetworkButtons.SetActive(true);
        AdsManager.Instance.GetAds(ADSText, ADS);
        gameObject.SetActive(true);
    }

    public void ShareFB()
    {
        FBManager.Instance.OpenShare("Kek");
    }

    public void ShareVK()
    {
        VKManager.Instance.OpenShare("Kek");
    }

    public void ShareOK()
    {
        OKManager.Instance.OpenShare("Kek");
    }
}
