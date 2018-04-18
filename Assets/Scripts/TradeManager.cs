using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class TradeManager : MonoBehaviour {
    
    public static TradeManager Instance { get; private set; }
    
    public string TradeItemsUrl = "/api/trade/tradeitems";
    public string OfferTradeUrl = "/api/trade/offertrade";
    public string AcceptTradeUrl = "/api/trade/accept";
    public string DeclineTradeUrl = "/api/trade/decline";
    public string TradeOffersUrl = "/api/trade/offers";

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
        TradeItemsUrl = ServerInfo.GetUrl(TradeItemsUrl);
        OfferTradeUrl = ServerInfo.GetUrl(OfferTradeUrl);
        AcceptTradeUrl = ServerInfo.GetUrl(AcceptTradeUrl);
        DeclineTradeUrl = ServerInfo.GetUrl(DeclineTradeUrl);
        TradeOffersUrl = ServerInfo.GetUrl(TradeOffersUrl);
    }
    public void GetTradeItemsAsync(int userID)
    {
        Canvaser.ShowLoading(true);
        InputInt input = new InputInt() { Value = userID };
        StartCoroutine(NetworkHelper.SendRequest(TradeItemsUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            TradeItemsModel info = JsonConvert.DeserializeObject<TradeItemsModel>(response.Text);
            Canvaser.Instance.SetTradeItems(info);
            Canvaser.ShowLoading(false);
        }));
    }

    public void OfferTradeAsync(TradeOfferModel offer)
    {
        StartCoroutine(NetworkHelper.SendRequest(OfferTradeUrl, offer, "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.TradeOffered();
            LoginManager.Instance.GetUserInfoAsync();
        }));
    }
    public void GetTradeOffersAsync()
    {
        StartCoroutine(NetworkHelper.SendRequest(TradeOffersUrl, null, "application/json", (response) =>
        {
            Debug.Log("OK");
            List<TradeOfferModel> info = JsonConvert.DeserializeObject<List<TradeOfferModel>>(response.Text);
            Canvaser.Instance.TradePanel.SetTrades(info);
            Canvaser.ShowLoading(false);
        }));
    }

    public void AcceptTradeAsync(int userID)
    {
        InputInt input = new InputInt() { Value = userID };
        StartCoroutine(NetworkHelper.SendRequest(AcceptTradeUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.TradePanel.Details.gameObject.SetActive(false);
            GetTradeOffersAsync();
        }));
    }
    public void DeclineTradeAsync(int userID)
    {
        InputInt input = new InputInt() { Value = userID };
        StartCoroutine(NetworkHelper.SendRequest(DeclineTradeUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.TradePanel.Details.gameObject.SetActive(false);
            GetTradeOffersAsync();
            LoginManager.Instance.GetUserInfoAsync();
        }));
    }
}

