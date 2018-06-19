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
        InputInt input = new InputInt() { Value = userID };
        CoroutineManager.SendRequest(TradeItemsUrl, input,  (TradeItemsModel info) =>
        {
            Canvaser.Instance.SetTradeItems(info);
        });
    }

    public void OfferTradeAsync(TradeOfferModel offer)
    {
        CoroutineManager.SendRequest(OfferTradeUrl, offer,  () =>
        {
            Canvaser.Instance.TradeOffered();
            //LoginManager.Instance.GetUserInfoAsync();
        });
    }
    public void GetTradeOffersAsync()
    {
        CoroutineManager.SendRequest(TradeOffersUrl, null,  (List<TradeOfferModel> info) =>
        {
            Canvaser.Instance.TradePanel.SetTrades(info);
        });
    }

    public void AcceptTradeAsync(int userID)
    {
        InputInt input = new InputInt() { Value = userID };
        CoroutineManager.SendRequest(AcceptTradeUrl, input,  () =>
        {
            Canvaser.Instance.TradePanel.Details.gameObject.SetActive(false);
            GetTradeOffersAsync();
        });
    }

    public void DeclineTradeAsync(int userID)
    {
        InputInt input = new InputInt() { Value = userID };
        CoroutineManager.SendRequest(DeclineTradeUrl, input,  () =>
        {
            Canvaser.Instance.TradePanel.Details.gameObject.SetActive(false);
            GetTradeOffersAsync();
            LoginManager.Instance.GetUserInfoAsync();
        });
    }
}

