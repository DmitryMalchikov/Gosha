using Newtonsoft.Json;
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
        if (!LoginManager.LocalUser)
        {
            ShopManager.Instance.BuyItemAsync(ItemID, ItemName.Contains("Upgrade"), price, () => gameObject.SetActive(false));
        }
        else
        {
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
              
                ShopManager.CurrentShop.BonusUpgrades.Find(bu => bu.Id == ItemID).Amount++;
                ShopManager.UpdateShopItems();
                LoginManager.User.IceCream -= price;
                GameController.Instance.LoadBonusesTime(LoginManager.User.BonusUpgrades);

                Canvaser.Instance.SetAllIceCreams(LoginManager.User.IceCream);

                ThreadHelper.RunNewThread(() =>
                {
                    //Extensions.SaveJsonData(DataType.Shop, JsonConvert.SerializeObject(ShopManager.CurrentShop));
                    Extensions.SaveJsonData(DataType.UserInfo, JsonConvert.SerializeObject(LoginManager.User));
                });
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

                ThreadHelper.RunNewThread(() =>
                {
                    Extensions.SaveJsonData(DataType.UserInfo, JsonConvert.SerializeObject(LoginManager.User));
                });
            }

            Canvaser.Instance.Shop.CheckBuyBtns();
            gameObject.SetActive(false);
        }        
    }
}
