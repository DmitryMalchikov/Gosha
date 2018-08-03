using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public bool activeInHierarchy
    {
        get
        {
            return gameObject.activeInHierarchy;
        }
    }
}
