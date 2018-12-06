﻿using System.Collections.Generic;
using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TradePanel : MonoBehaviour
    {
        public int SelectedItemID;
        public int IcecreamForTrade;

        public ToggleGroup Toggles;

        public Transform CardsContent;
        public GameObject TradeCard;

        public List<ItemInfo> Cards;
        public List<ItemInfo> Bonuses;

        public Text IceCream;

        public TradeItemsModel info;

        public Text Title;

        public InputField IceCreamForTradeInput;

        public void SetContent(TradeItemsModel items)
        {
            ClearContent();
            gameObject.SetActive(true);
            info = items;
            if (Title.text.Contains(LocalizationManager.GetLocalizedValue("tradewith")))
                Title.text = LocalizationManager.GetLocalizedValue("tradewith") + Canvaser.Instance.FriendsPanel.TraderFriend.Nickname;
            foreach (InventoryCard item in items.Cards)
            {
                ItemInfo newItem = Instantiate(TradeCard, CardsContent).GetComponent<ItemInfo>();
                newItem.SetInventoryCard(item);
                newItem._TradePanel = this;
                newItem.GetComponent<Toggle>().group = Toggles;
            }
            for (int i = 0; i < items.Bonuses.Length; i++)
            {
                Bonuses.Find(x => x.ItemName == items.Bonuses[i].Name).SetBonus(items.Bonuses[i]);
            }
            IceCream.text = items.IceCream.Amount.ToString();
        }

        public void InputIceCream(string input)
        {
            Toggles.SetAllTogglesOff();
            SelectedItemID = info.IceCream.ItemId;
            IcecreamForTrade = int.Parse(input);
        }

        public void Continue()
        {
            Canvaser.Instance.ContinueTrade(new InventoryItem() { ItemId = SelectedItemID, Amount = (IcecreamForTrade == 0) ? 1 : IcecreamForTrade });
            gameObject.SetActive(false);
        }

        public void SendOffer()
        {
            Canvaser.Instance.SendOffer(new InventoryItem() { ItemId = SelectedItemID, Amount = (IcecreamForTrade == 0) ? 1 : IcecreamForTrade });
            gameObject.SetActive(false);
        }
        public void ClearContent()
        {
            CardsContent.ClearContent();
            Cards.Clear();

            foreach (ItemInfo item in Bonuses)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
