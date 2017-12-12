using System.Collections;
using System.Collections.Generic;
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

    }

	public void ShowTradeDetails()
    {
        Canvaser.Instance.TradePanel.Details.SetDetails(info);
    }
}
