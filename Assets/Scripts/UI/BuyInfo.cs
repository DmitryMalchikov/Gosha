using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuyInfo : MonoBehaviour
{
    public Text Title;
    public Image Sprite;
    public Text Description;
    public Text Price;
    public int ItemID { get; private set; }
    public string ItemName { get; private set; }
    public int PriceValue { get; private set; }

    public void SetBuyInfo(ItemInfo info)
    {
        ItemID = info.ItemID;
        Title.text = info.NameText.text;
        Sprite.sprite = info.ImgSource.sprite;
        Description.text = info.Desc;
        Price.text = info.PriceText.text;
        ItemName = info.ItemName;
        PriceValue = info.Price;
        gameObject.SetActive(true);
    }

    public void BuyItem()
    {
        int price = int.Parse(Price.text);
        if (!LoginManager.LocalUser)
        {
            ShopManager.Instance.BuyItemAsync(this, () => gameObject.SetActive(false));
        }
        else
        {
            BuyItemLocaly();
        }
    }

    public void BuyItemLocaly()
    {
        int price = int.Parse(Price.text);
        if (ItemName.Contains("Upgrade"))
        {
            var bonusUpgrade = LoginManager.User.BonusUpgrades.Find(bu => bu.BonusName == ItemName);

            if (bonusUpgrade == null)
            {
                LoginManager.User.BonusUpgrades.Add(new BonusUpgrade() { BonusName = ItemName, UpgradeAmount = 1 });
            }
            else
            {
                bonusUpgrade.UpgradeAmount++;
            }

            ShopManager.CurrentShop.BonusUpgrades.FirstOrDefault(bu => bu.Id == ItemID).Amount++;
            ShopManager.UpdateShopItems();
            LoginManager.User.IceCream -= price;
            GameController.Instance.LoadBonusesTime(LoginManager.User.BonusUpgrades);

            Canvaser.Instance.SetAllIceCreams(LoginManager.User.IceCream);

            FileExtensions.SaveJsonDataAsync(DataType.UserInfo, LoginManager.User);
        }
        else if (!string.IsNullOrEmpty(ItemName))
        {
            Bonus bonus = LoginManager.User.Bonuses.Find(b => b.Name.Name == ItemName);
            if (bonus == null)
            {
                LoginManager.User.Bonuses.Add(new Bonus() { Name = new NameLocalization() { Name = ItemName }, Amount = 1 });
            }
            else
            {
                bonus.Amount += 1;
            }

            LoginManager.User.IceCream -= price;
            Canvaser.Instance.SetAllIceCreams(LoginManager.User.IceCream);
            Canvaser.Instance.SBonuses.SetStartBonuses(LoginManager.User.Bonuses);

            FileExtensions.SaveJsonDataAsync(DataType.UserInfo, LoginManager.User);
        }

        Canvaser.Instance.Shop.CheckBuyBtns();
        gameObject.SetActive(false);
    }
}
