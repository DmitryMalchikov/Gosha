using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyInfo : MonoBehaviour {

    public Text Title;
    public Image Sprite;
    public Text Description;
    public Text Price;
    public int ItemID;
    public string ItemName;

    public void SetBuyInfo(int itemID, string title, string desc, string price, Sprite img, string itemName)
    {
        ItemID = itemID;
        Title.text = title;
        Sprite.sprite = img;
        Description.text = desc;
        Price.text = price;
        ItemName = itemName;
        gameObject.SetActive(true);
    }

    public void BuyItem()
    {
        int price = int.Parse(Price.text);
        ShopManager.Instance.BuyItemAsync(ItemID, ItemName.Contains("Upgrade"), price, () => gameObject.SetActive(false));
        
    }
}
