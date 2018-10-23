using UnityEngine;

public class LoginCanvas : Singleton<LoginCanvas>
{
    public GameObject Warning;

    public void Enable(bool enable)
    {
        EnableWarning(false);
        gameObject.SetActive(enable);
        Canvaser.Instance.MainMenu.SetActive(!enable);        
    }

    public void EnableWarning(bool enable)
    {
        Warning.SetActive(enable);
    }
}
