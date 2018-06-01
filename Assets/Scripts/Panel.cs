using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public List<GameObject> LoadingPanels = new List<GameObject>();

    private RectTransform panelRectTransform;

    void OnEnable()
    {
        StartCoroutine(WaitCanvas());
    }

    IEnumerator WaitCanvas()
    {
        yield return new WaitUntil(() => Canvaser.Instance != null);

        panelRectTransform = transform as RectTransform;
        panelRectTransform.SetAsLastSibling();
        if (Canvaser.Instance.ErrorWindow.activeInHierarchy)
        {
            Canvaser.Instance.ErrorWindow.transform.SetAsLastSibling();
        }
        else if (Canvaser.Instance.LoadingPanel.activeInHierarchy)
        {
            Canvaser.Instance.LoadingPanel.transform.SetAsLastSibling();
        }
    }
}
