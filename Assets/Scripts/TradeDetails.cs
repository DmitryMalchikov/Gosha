using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeDetails : MonoBehaviour {

    public TradeOfferModel info;

    public Text Title;
    public Button AcceptBtn;
    public Button DeclineBtn;

    public Text FirstUserName;
    public Text SecondUserName;
    
    public Image FirstUserItemImg;
    public Image SecondUserItemImg;

    public Text FirstUserItemName;
    public Text SecondUserItemName;

    public void Accept()
    {
        TradeManager.Instance.AcceptTradeAsync(info.Id);
    }

    public void Decline()
    {
        TradeManager.Instance.DeclineTradeAsync(info.Id);
    }

    public void SetDetails(TradeOfferModel model)
    {
        info = model;

        if(model.UserId == LoginManager.Instance.User.Id)
        {
            Title.text = LocalizationManager.GetLocalizedValue("yourtradeoffer") + model.Nickname;
            FirstUserName.text = LoginManager.Instance.User.Nickname;
            SecondUserName.text = model.Nickname;
            AcceptBtn.gameObject.SetActive(false);
        }
        else
        {
            Title.text = model.Nickname + LocalizationManager.GetLocalizedValue("offeredyou");
            FirstUserName.text = model.Nickname;
            SecondUserName.text = LoginManager.Instance.User.Nickname;
            AcceptBtn.gameObject.SetActive(true);
        }

        FirstUserItemName.text = model.OfferItem.Name;
        if(model.OfferItem.Name == "Ice cream")
        {
            FirstUserItemName.text += ": " + model.OfferItem.Amount;
        }
        SecondUserItemName.text = model.RequestItem.Name;
        if(model.RequestItem.Name == "Ice cream")
        {
            SecondUserItemName.text += ": " + model.RequestItem.Amount;
        }
        gameObject.SetActive(true);
    }
}
