using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Button LogOutBtn;
    public Button LogInBtn;
    public Button ResetPassBtn;

    public void Open(bool inMainMenu)
    {
        ResetPassBtn.gameObject.SetActive(!LoginManager.LocalUser);
        LogOutBtn.gameObject.SetActive(!LoginManager.LocalUser);
        LogInBtn.gameObject.SetActive(LoginManager.LocalUser);
        Canvaser.Instance.SettingsAvatar.gameObject.SetActive(!LoginManager.LocalUser);
        Canvaser.Instance.SettingsRegion.gameObject.SetActive(!LoginManager.LocalUser);
        Canvaser.Instance.SettingsRegionWord.gameObject.SetActive(!LoginManager.LocalUser);
        Canvaser.Instance.SettingsAvatarBorder.gameObject.SetActive(!LoginManager.LocalUser);
        Canvaser.Instance.SettingsNickname.gameObject.SetActive(!LoginManager.LocalUser);

        LogOutBtn.interactable = inMainMenu;
        LogInBtn.interactable = inMainMenu;
        ResetPassBtn.interactable = inMainMenu;
        gameObject.SetActive(true);
    }
}
