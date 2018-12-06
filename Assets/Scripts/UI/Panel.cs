using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class Panel : MonoBehaviour
    {
        private RectTransform _panelRectTransform;

        void OnEnable()
        {
            StartCoroutine(WaitCanvas());
        }

        IEnumerator WaitCanvas()
        {
            yield return new WaitUntil(() => Canvaser.Instance != null);

            _panelRectTransform = transform as RectTransform;
            _panelRectTransform.SetAsLastSibling();
            if (Canvaser.Instance.ErrorWindow.activeInHierarchy)
            {
                Canvaser.Instance.ErrorWindow.transform.SetAsLastSibling();
                Canvaser.Instance.MainPanel.SetAsLastSibling();
            }
            else if (Canvaser.Instance.LoadingPanel.activeInHierarchy)
            {
                Canvaser.Instance.LoadingPanel.transform.SetAsLastSibling();
            }
        }
    }
}
