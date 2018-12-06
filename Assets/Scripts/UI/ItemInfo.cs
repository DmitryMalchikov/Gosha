using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ItemInfo : MonoBehaviour
    {
        public string ItemName;
        public string Desc;
        private ShopItem _itemInfo;
        public Image ImgSource;
        public Text NameText;
        public Text PriceText;
        public Button BuyButton;
        public TradePanel _TradePanel;
        public Slider Upgrade;

        public int Price { get { return _itemInfo.Cost; } }

        public int ItemId
        {
            get
            {
                var item = _itemInfo as InventoryItem;
                if (item != null)
                {
                    return item.ItemId;
                }
                return _itemInfo.Id;
            }
        }

        public void OpenBuyInfo()
        {
            Canvaser.Instance.BuyInfoPanel.SetBuyInfo(this);
        }
        public void OpenUpgradeInfo()
        {
            Canvaser.Instance.UpgradeInfoPanel.SetBuyInfo(this);
        }

        public void SetCard(ShopCard card)
        {
            SetItemAndText(card);
            ImgSource.sprite = Resources.Load<Sprite>(string.Format("{0} ({1})", card.SuitName, card.Position));
        }

        public void SetCase(ShopItem item)
        {
            SetItemAndText(item);
        }

        public void SetItemAndText(ShopItem item)
        {
            _itemInfo = item;
            NameText.text = LocalizationManager.GetValue(item.NameRu, item.Name);
            PriceText.text = item.Cost.ToString();
        }

        public void SetUpgrade(ShopItem item)
        {
            int amount = item.Amount;
            _itemInfo = item;

            if (LoginManager.LocalUser)
            {
                var upgrade = LoginManager.User.BonusUpgrades.Find(bu => bu.BonusName == item.Name);
                amount = upgrade != null ? upgrade.UpgradeAmount : 0;
            }

            Upgrade.value = amount;
            if (amount < 5)
            {
                _itemInfo.Cost = (item.Cost * (amount + 1));
                PriceText.text = Price.ToString();
                BuyButton.interactable = LoginManager.User.IceCream >= (item.Cost * (amount + 1));
            }
            else
            {
                BuyButton.interactable = false;
                PriceText.text = "Max";
            }
        }

        public void SetBonus(ShopItem item)
        {
            _itemInfo = item;
            PriceText.text = item.Cost.ToString();
        }

        public void SelectItem(bool toSelect)
        {
            if (toSelect)
            {
                _TradePanel.SelectedItemID = ItemId;
                _TradePanel.IcecreamForTrade = 0;
                _TradePanel.IceCreamForTradeInput.text = "";
            }
            NameText.text = string.Format("{0}({1})", LocalizationManager.GetValue(_itemInfo.NameRu, _itemInfo.Name), _itemInfo.Amount - 1 * (toSelect ? 1 : 0));
        }

        public void SetInventoryCard(InventoryCard item)
        {
            _itemInfo = item;
            NameText.text = string.Format("{0}({1})", LocalizationManager.GetValue(item.NameRu, item.Name), item.Amount);
            ImgSource.sprite = Resources.Load<Sprite>(item.Name.AddBrackets());
        }
        public void SetBonus(InventoryItem item)
        {
            gameObject.SetActive(true);
            _itemInfo = item;
            NameText.text = string.Format("{0}({1})", LocalizationManager.GetValue(item.NameRu, item.Name), item.Amount);
        }
    }
}
