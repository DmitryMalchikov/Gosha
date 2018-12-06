using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ShowTradesPanel : MonoBehaviour
    {
        public Text Title;
        public Transform Content;
        public GameObject TradeInfoObject;
        public TradeDetails Details;

        public void SetTrades(TradeOfferModel[] trades)
        {
            ClearContent();
            if (trades.Length == 0)
            {
                Title.text = LocalizationManager.GetLocalizedValue("nooffers");
            }
            else
            {
                Title.text = trades.Length + LocalizationManager.GetLocalizedValue("tradeoffers");
                foreach (TradeOfferModel item in trades)
                {
                    TradeInfo newTrade = Instantiate(TradeInfoObject, Content).GetComponent<TradeInfo>();
                    newTrade.SetTrade(item);
                }
            }
            gameObject.SetActive(true);
        }

        public void ClearContent()
        {
            Content.ClearContent();
        }
    }
}
