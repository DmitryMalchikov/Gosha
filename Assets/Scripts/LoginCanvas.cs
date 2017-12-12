using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginCanvas : MonoBehaviour {

    public static LoginCanvas Instance { get; private set; }

    public GameObject Warning;

    private void Awake()
    {
        Instance = this;
    }

    public void Enable(bool enable)
    {
        gameObject.SetActive(enable);
        Canvaser.Instance.MainMenu.SetActive(!enable);
    }

    public void EnableWarning(bool enable)
    {
        Warning.SetActive(enable);
    }
}
