using UnityEngine;
using UnityEngine.UI;

public class TradeInfo : MonoBehaviour {

    public Text Title;
    public Image Item1;
    public Image Item2;

    public TradeOfferModel info;
    

    public void SetTrade(TradeOfferModel model)
    {
        info = model;
        if(model.UserId == LoginManager.Instance.User.Id)
        {
            Title.text = string.Format(LocalizationManager.GetLocalizedValue("youoffered"), model.Nickname);
        }
        else
        {
            Title.text = model.Nickname + LocalizationManager.GetLocalizedValue("offeredyoutrade");
        }
        if (model.OfferItem.Name.Contains("Card"))
        {
            Item1.sprite = Resources.Load<Sprite>(Canvaser.Instance.AddBrackets(model.OfferItem.Name));
        }
        else
        {
            Item1.sprite = Resources.Load<Sprite>("Bonus" + model.OfferItem.ItemId);
        }
        if (model.RequestItem.Name.Contains("Card"))
        {
            Item2.sprite = Resources.Load<Sprite>(Canvaser.Instance.AddBrackets(model.RequestItem.Name));
        }
        else
        {
            Item2.sprite = Resources.Load<Sprite>("Bonus" + model.RequestItem.ItemId);
        }
    }

	public void ShowTradeDetails()
    {
        Canvaser.Instance.TradePanel.Details.SetDetails(info);
    }
}
