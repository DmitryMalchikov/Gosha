﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfo : MonoBehaviour
{
    public FriendObject info;

    public Text Name;
    public Text Region;
    public Text Record;
    public Text Duels;
    public Text IceCreamCount;
    public Text AddToFriendsText;
    public Button AddToFriendsBtn;
    public Image Avatar;
    public Button TradeBtn;


    public void SetInfo(FriendObject user, bool isFriend = true)
    {
        info = user;
        Name.text = user.Info.Nickname;
        Region.text = user.Info.Region;
        Record.text = LocalizationManager.GetLocalizedValue("highscore") + user.Info.Highscore.ToString();
        Duels.text = LocalizationManager.GetLocalizedValue("duelwins") + user.Info.DuelWins.ToString();
        IceCreamCount.text = user.Info.IceCream.ToString();
        Avatar.sprite = user.Avatar.sprite;
        //Avatar.SetNativeSize();
        if (isFriend)
        {
            TradeBtn.interactable = LoginManager.User.CanOfferTrade;
        }
        gameObject.SetActive(true);
    }

    public void SetOfferInfo(FriendObject user, bool requestSent = false)
    {
        info = user;
        Name.text = user.OfferInfo.Nickname;
        Region.text = user.OfferInfo.Region;
        Record.text = LocalizationManager.GetLocalizedValue("highscore") + user.OfferInfo.Highscore.ToString();
        Duels.text = LocalizationManager.GetLocalizedValue("duelwins") + user.OfferInfo.DuelWins.ToString();
        IceCreamCount.text = user.OfferInfo.IceCream.ToString();
        Avatar.sprite = user.Avatar.sprite;
        SetAddToFriendsButton(requestSent);

        gameObject.SetActive(true);
    }

    private void SetAddToFriendsButton(bool alreadySent)
    {
        if (alreadySent)
        {
            AddToFriendsBtn.interactable = false;
            AddToFriendsText.text = LocalizationManager.GetLocalizedValue("friendrequestalreadysend");
        }
        else if (AddToFriendsBtn)
        {
            AddToFriendsBtn.interactable = true;
            AddToFriendsText.text = LocalizationManager.GetLocalizedValue("addtofriends");
        }
    }

    public void OfferOrAddFriend()
    {
        if (Canvaser.Instance.IsLoggedIn())
        {
            int newStatus = info.OfferOrAddfriend();
            if (newStatus == 3)
            {
                SetAddToFriendsButton(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void AddFriend(bool toAdd)
    {
        info.AcceptFriend(toAdd);
        gameObject.SetActive(false);
    }

    public void AcceptOffer(bool toAdd)
    {
        info.AcceptOffer(toAdd);
        gameObject.SetActive(false);
    }

    public void AcceptFriend(bool toAdd)
    {
        info.AcceptFriend(toAdd);
        gameObject.SetActive(false);
    }

    public void RemoveFriend()
    {
        info.AcceptFriend(false);
        gameObject.SetActive(false);
    }
    public void Trade()
    {
        info.Trade();
        gameObject.SetActive(false);
    }

    public void Duel()
    {
        if (Canvaser.Instance.IsLoggedIn())
        {
            if (info.Info == null)
            {
                Canvaser.Instance.FriendsPanel.OpenDuelPanel(info.OfferInfo.Id);
            }
            else
            {
                Canvaser.Instance.FriendsPanel.OpenDuelPanel(info.Info.Id);
            }
            gameObject.SetActive(false);
        }
    }
}
