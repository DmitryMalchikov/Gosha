using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTradesPanel : MonoBehaviour {

    public Text Title;
    public Transform Content;
    public GameObject TradeInfoObject;

    public List<TradeInfo> Trades;
    public TradeDetails Details;

    public void SetTrades(List<TradeOfferModel> trades)
    {
        ClearContent();
        if (trades.Count == 0)
        {
            Title.text = LocalizationManager.GetLocalizedValue("nooffers");
        }
        else
        {
            Title.text = trades.Count + LocalizationManager.GetLocalizedValue("tradeoffers");
            foreach (TradeOfferModel item in trades)
            {
                TradeInfo newTrade = Instantiate(TradeInfoObject, Content).GetComponent<TradeInfo>();
                newTrade.SetTrade(item);
                Trades.Add(newTrade);
            }
        }
        gameObject.SetActive(true);
    }

    public void ClearContent()
    {
        foreach (Transform item in Content)
        {
            Destroy(item.gameObject);
        }
        Trades.Clear();
    }
}
