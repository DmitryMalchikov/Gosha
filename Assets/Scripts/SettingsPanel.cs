
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour {

    [SerializeField]
    GameObject logOutBtn;

    public void Open(bool withLogOut)
    {
        logOutBtn.SetActive(withLogOut);
        gameObject.SetActive(true);
    }
}
