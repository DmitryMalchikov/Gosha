using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }
    public static string PanelKeyToEnable = null; 

    public List<LoadingPanel> LoadingPanels;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(PanelKeyToEnable))
        {
            Extensions.ShowGameObjects(GetPanelsByKey(PanelKeyToEnable));
            PanelKeyToEnable = null;
        }
    }

    public static List<GameObject> GetPanelsByKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return new List<GameObject>();
        }
        return Instance.LoadingPanels.Where(lp => lp.Key == key).Select(lp => lp.Panel).ToList();
    }
}

[System.Serializable]
public class LoadingPanel
{
    public GameObject Panel;
    public string Key;
}