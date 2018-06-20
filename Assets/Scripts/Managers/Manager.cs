using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public void ShowLoadingPanels()
    {
        for (int i = 0; i < LoadingManager.Instance.LoadingPanels.Count; i++)
        {
            LoadingManager.Instance.LoadingPanels[i].Panel.SetActive(true);
        }
    }


}

