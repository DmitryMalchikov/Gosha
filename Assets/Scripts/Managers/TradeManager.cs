public class TradeManager : APIManager<TradeManager>
{
    public string TradeItemsUrl = "/api/trade/tradeitems";
    public string OfferTradeUrl = "/api/trade/offertrade";
    public string AcceptTradeUrl = "/api/trade/accept";
    public string DeclineTradeUrl = "/api/trade/decline";
    public string TradeOffersUrl = "/api/trade/offers";

    public override void SetUrls()
    {
        ServerInfo.SetUrl(ref TradeItemsUrl);
        ServerInfo.SetUrl(ref OfferTradeUrl);
        ServerInfo.SetUrl(ref AcceptTradeUrl);
        ServerInfo.SetUrl(ref DeclineTradeUrl);
        ServerInfo.SetUrl(ref TradeOffersUrl);
    }
    public void GetTradeItemsAsync(int userID)
    {
        InputInt input = new InputInt() { Value = userID };
        CoroutineManager.SendRequest(TradeItemsUrl, input, (TradeItemsModel info) =>
       {
           Canvaser.Instance.SetTradeItems(info);
       });
    }

    public void OfferTradeAsync(TradeOfferModel offer)
    {
        CoroutineManager.SendRequest(OfferTradeUrl, offer, () =>
       {
           Canvaser.Instance.TradeOffered();
            //LoginManager.Instance.GetUserInfoAsync();
        });
    }
    public void GetTradeOffersAsync()
    {
        CoroutineManager.SendRequest(TradeOffersUrl, null, (TradeModel info) =>
       {
           Canvaser.Instance.TradePanel.SetTrades(info.Trades);
           GameController.SetHash("TradesHash", info.TradesHash);
       }, loadingPanelsKey: "trades", type: DataType.Trades);
    }

    public void AcceptTradeAsync(int userID)
    {
        InputInt input = new InputInt() { Value = userID };
        CoroutineManager.SendRequest(AcceptTradeUrl, input, () =>
       {
           Canvaser.Instance.TradePanel.Details.gameObject.SetActive(false);
           GetTradeOffersAsync();
       });
    }

    public void DeclineTradeAsync(int userID)
    {
        InputInt input = new InputInt() { Value = userID };
        CoroutineManager.SendRequest(DeclineTradeUrl, input, () =>
       {
           Canvaser.Instance.TradePanel.Details.gameObject.SetActive(false);
           GetTradeOffersAsync();
           LoginManager.Instance.GetUserInfoAsync();
       });
    }
}

