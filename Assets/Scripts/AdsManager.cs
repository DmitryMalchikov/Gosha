﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour {

    public event ResultCallback OnAdsDownloaded;
    public static AdsManager Instance { get; private set; }

    public Image AdsImage;
    public Text AdsText;

    public string AdsUrl = "/api/ads/Advertisement";
    public string AdsImageUrl = "/api/ads/adsimage?adsId=";
	public string DoubleScoreUrl = "/api/gameplay/doublescore";

    private List<GameObject> _loadingPanels;

	private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetUrls();
    }
    public void SetUrls()
    {
        AdsUrl = ServerInfo.GetUrl(AdsUrl);
        AdsImageUrl = ServerInfo.GetUrl(AdsImageUrl);
		DoubleScoreUrl = ServerInfo.GetUrl(DoubleScoreUrl);
    }

	public void DoubleScoreAds(Text text, Image image)
	{
		StartCoroutine(NetworkHelper.SendRequest(DoubleScoreUrl, new { Value = (int)LocalizationManager.CurrentLanguage }, "application/json", (response) =>
		{
			AdsModel ads = JsonConvert.DeserializeObject<AdsModel>(response.Text);
			text.text = ads.Text;

			GetAdsImage(ads.Id, image);
		}));
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

    public void GetAds(Text text, Image image)
    {
        Canvaser.Instance.ADSPanel.OpenAds();
        image.sprite = null;
        TurnOnLoading();
        StartCoroutine(NetworkHelper.SendRequest(AdsUrl, new { Value = (int)LocalizationManager.CurrentLanguage}, "application/json", (response) => 
        {
            AdsModel ads = JsonConvert.DeserializeObject<AdsModel>(response.Text);
            text.text = ads.Text;

            GetAdsImage(ads.Id, image);
        }));
    }

    public void GetAdsImage(int adsId, Image image)
    {
        StartCoroutine(DownloadImage(adsId, image));
    }

    IEnumerator DownloadImage(int adsId, Image image)
    {
        NetworkHelper.StartRequest();
        string url = AdsImageUrl + adsId;

        WWW www = new WWW(url);

        yield return www;

        image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f);
        if (OnAdsDownloaded != null)
        {
            OnAdsDownloaded();
        }
        OnAdsDownloaded = null;
        NetworkHelper.StopRequest();
        TurnOffLoading();
    }

}
