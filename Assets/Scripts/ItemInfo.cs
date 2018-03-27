using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{

    public int ItemID;
    public string ItemName;
    public string Desc;

    public ShopCard CardInfo;
    public ShopItem CaseInfo;

    public Image ImgSource;
    public Text NameText;
    public Text PriceText;
    public Button BuyButton;

    public TradePanel _TradePanel;

    public Slider Upgrade;

	InventoryItem curItem;

    public void OpenBuyInfo()
    {
        Canvaser.Instance.BuyInfoPanel.SetBuyInfo(ItemID, NameText.text, Desc, PriceText.text, ImgSource.sprite, ItemName);
    }
    public void OpenUpgradeInfo()
    {
        Canvaser.Instance.UpgradeInfoPanel.SetBuyInfo(ItemID, NameText.text, Desc, PriceText.text, ImgSource.sprite, ItemName);
    }

    public void SetCard(ShopCard card)
    {
        CardInfo = card;
        NameText.text = card.Name;
        PriceText.text = card.Cost.ToString();
        ItemID = card.Id;
		ImgSource.sprite = Resources.Load<Sprite> (string.Format("{0} ({1})", CardInfo.SuitName, CardInfo.Position));
    }

    public void SetCase(ShopItem item)
    {
        CaseInfo = item;
        NameText.text = item.Name;
        PriceText.text = item.Cost.ToString();
        ItemID = item.Id;
    }

    public void SelectItem(bool toSelect)
    {
        if(toSelect)
        {
            _TradePanel.SelectedItemID = ItemID;
            _TradePanel.IcecreamForTrade = 0;
            _TradePanel.IceCreamForTradeInput.text = "";
		}
		NameText.text = string.Format("{0}({1})", curItem.Name, curItem.Amount - 1 * (toSelect ? 1 : 0));
	}

    public void SetInventoryCard(InventoryCard item)
    {
        ItemID = item.ItemId;
        NameText.text = string.Format("{0}({1})", item.Name, item.Amount);
    }
    public void SetBonus(InventoryItem item)
    {
        gameObject.SetActive(true);
		curItem = item;
        NameText.text = string.Format("{0}({1})", item.Name, item.Amount);
    }
}
