using UnityEngine;
using UnityEngine.UI;

public class AdsPanel : MonoBehaviour {

    public Image img;
    public Text txt;

    public void OpenAds()
    {
        gameObject.SetActive(true);
        Canvaser.Instance.DoubleIcecreamClicked = false;
    }

	public void DoubleScore()
	{
		//Canvaser.Instance.LoadingPanel.SetActive(true);
		AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.ADSPanel.OpenAds();
		//AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.CloseLoading();
		AdsManager.Instance.DoubleScoreAds(txt, img);
	}
}
