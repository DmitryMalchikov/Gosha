using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class AdsPanel : MonoBehaviour {

        public RawImage img;
        public Text txt;

        public List<GameObject> LoadingPanels = new List<GameObject>();

        public void OpenAds()
        {
            gameObject.SetActive(true);
            Canvaser.Instance.DoubleIcecreamClicked = false;
        }

        public void OpenUrl()
        {
            Application.OpenURL(txt.text);
        }

        public void DoubleScore()
        {
            AdsManager.Instance.GetAds(txt, img);
        }
    }
}
