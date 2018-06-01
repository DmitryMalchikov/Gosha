using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    public static List<GameObject> LoadingPanels(this MonoBehaviour element)
    {
        return element.GetComponent<Panel>().LoadingPanels;
    }

    public static List<GameObject> LoadingPanels(this MonoBehaviour element, int index)
    {
        return new List<GameObject>{element.GetComponent<Panel>().LoadingPanels[index]};
    }

    public static List<GameObject> LoadingPanels(this GameObject element)
    {
        return element.GetComponent<Panel>().LoadingPanels;
    }
}
