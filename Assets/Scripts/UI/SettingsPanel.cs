using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SettingsPanel : MonoBehaviour
    {
        public Button LogOutBtn;
        public Button LogInBtn;
        public Button ResetPassBtn;

        public GameObject SettingsRegionWord;
        public GameObject SettingsAvatarBorder;
        public GameObject SettingsNickname;

        public void Open(bool inMainMenu)
        {
            ResetPassBtn.gameObject.SetActive(!LoginManager.LocalUser);
            LogOutBtn.gameObject.SetActive(!LoginManager.LocalUser);
            LogInBtn.gameObject.SetActive(LoginManager.LocalUser);

            Canvaser.Instance.SettingsAvatar.gameObject.SetActive(!LoginManager.LocalUser);
            Canvaser.Instance.SettingsRegion.gameObject.SetActive(!LoginManager.LocalUser);

            SettingsRegionWord.SetActive(!LoginManager.LocalUser);
            SettingsAvatarBorder.SetActive(!LoginManager.LocalUser);
            SettingsNickname.SetActive(!LoginManager.LocalUser);

            LogOutBtn.interactable = inMainMenu;
            LogInBtn.interactable = inMainMenu;
            ResetPassBtn.interactable = inMainMenu;
            gameObject.SetActive(true);
        }
    }
}
