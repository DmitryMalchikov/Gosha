using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsPanel : MonoBehaviour {

    public Image img;
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
		//Canvaser.Instance.LoadingPanel.SetActive(true);
		//AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.ADSPanel.OpenAds();
		AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.CloseLoading();
		AdsManager.Instance.GetAds(txt, img);
	}
}
