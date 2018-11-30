using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsManager : APIManager<AdsManager>
{
    public event ResultCallback OnAdsDownloaded;
    public static bool Initialized = false;

    public string AdsUrl = "/api/ads/Advertisement";
    public string AdsImageUrl = "/api/ads/adsimage?adsId=";
	public string DoubleScoreUrl = "/api/gameplay/doublescore";

    private List<GameObject> _loadingPanels;

    public override void SetUrls()
    {
        ServerInfo.SetUrl(ref AdsUrl);
        ServerInfo.SetUrl(ref AdsImageUrl);
		ServerInfo.SetUrl(ref DoubleScoreUrl);
        Initialized = true;
    }

	public void DoubleScoreAds(Text text, RawImage image)
	{
        CoroutineManager.SendRequest(DoubleScoreUrl, new { Value = (int)LocalizationManager.CurrentLanguage },  (AdsModel ads) =>
		{
			text.text = ads.Text;

			GetAdsImage(ads.Id, image);
		});
	}

    private void TurnOnLoading()
    {
        if (_loadingPanels == null)
        {
            _loadingPanels = Canvaser.Instance.ADSPanel.GetComponent<AdsPanel>().LoadingPanels;
        }

        for (int i = 0; i < _loadingPanels.Count; i++)
        {
            _loadingPanels[i].SetActive(true);
        }
    }

    private void TurnOffLoading()
    {
        for (int i = 0; i < _loadingPanels.Count; i++)
        {
            _loadingPanels[i].SetActive(false);
        }
    }

    public void GetAds(Text text, RawImage image)
    {
        Canvaser.Instance.ADSPanel.OpenAds();
        image.texture = null;
        TurnOnLoading();
        CoroutineManager.SendRequest(AdsUrl, new { Value = (int)LocalizationManager.CurrentLanguage},  (AdsModel ads) => 
        {
            text.text = ads.Text;
            GetAdsImage(ads.Id, image);
        }, 
        errorMethod: (model) =>
        {
            Canvaser.Instance.ADSPanel.gameObject.SetActive(false);
            OnAdsDownloaded();
        }
        );
    }

    public void GetAdsImage(int adsId, RawImage image)
    {
        StartCoroutine(DownloadImage(adsId, image));
    }

    IEnumerator DownloadImage(int adsId, RawImage image)
    {
        string url = AdsImageUrl + adsId;

        WWW www = new WWW(url);

        yield return www;

        image.texture = www.texture;
        if (OnAdsDownloaded != null)
        {
            OnAdsDownloaded();
        }
        OnAdsDownloaded = null;
        TurnOffLoading();
    }

}
