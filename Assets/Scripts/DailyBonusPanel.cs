using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBonusPanel : MonoBehaviour
{

    public List<GameObject> Highlights;
    public GameObject GetBonusButton;
    public GameObject BackButton;

    public void SetHighlights()
    {
        Debug.Log(LoginManager.Instance.User.DaysInRow);
        TurnOffHighlights();
        for (int i = 0; i < LoginManager.Instance.User.DaysInRow + 1; i++)
        {
            Highlights[i].SetActive(true);
        }
        GetBonusButton.SetActive(!LoginManager.Instance.User.GotDailyBonus);
        BackButton.SetActive(LoginManager.Instance.User.GotDailyBonus);
    }
    public void TurnOffHighlights()
    {
        for (int i = 0; i < LoginManager.Instance.User.DaysInRow + 1; i++)
        {
            if (Highlights[i].activeInHierarchy)
                Highlights[i].SetActive(false);
        }
    }
}
