using System.Collections;
using UnityEngine;

public class Panel : MonoBehaviour
{
    private RectTransform panelRectTransform;

    void OnEnable()
    {
        StartCoroutine(WaitCanvas());
    }

    IEnumerator WaitCanvas()
    {
        yield return new WaitUntil(() => Canvaser.Instance != null);

        panelRectTransform = transform as RectTransform;
        if (Canvaser.Instance.LoadingPanel.activeInHierarchy)
        {
            Canvaser.Instance.LoadingPanel.transform.SetAsLastSibling();
        }
        else
        {
            panelRectTransform.SetAsLastSibling();
        }
    }
}
