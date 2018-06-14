using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {
    
    public Button LogOutBtn;
    public Button ResetPassBtn;

    public void Open(bool inMainMenu)
    {
        LogOutBtn.interactable = inMainMenu;
        ResetPassBtn.interactable = inMainMenu;
        gameObject.SetActive(true);
    }
}
