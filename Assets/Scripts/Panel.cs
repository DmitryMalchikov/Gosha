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
        panelRectTransform.SetAsLastSibling();
        if (Canvaser.Instance.ErrorWindow.activeInHierarchy)
        {
            Canvaser.Instance.ErrorWindow.transform.SetAsLastSibling();
        }
        else if (Canvaser.Instance.LoadingPanel.activeInHierarchy)
        {
            Canvaser.Instance.LoadingPanel.transform.SetAsLastSibling();
        }
        else if (Canvaser.Instance.GameOverPanel.activeInHierarchy)
        {
            Canvaser.Instance.GameOverPanel.transform.SetAsLastSibling();
        }
    }
}
