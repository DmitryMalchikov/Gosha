using System.Linq;
using Assets.Scripts.DTO;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class BuyInfo : MonoBehaviour
    {
        public Text Title;
        public Image Sprite;
        public Text Description;
        public Text Price;
        public int ItemId { get; private set; }
        public string ItemName { get; private set; }
        public int PriceValue { get; private set; }

        public void SetBuyInfo(ItemInfo info)
        {
            ItemId = info.ItemId;
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
            if (string.IsNullOrEmpty(ItemName))
            {
                return;
            }
        
            if (ItemName.Contains("Upgrade"))
            {
                BuyUpgradeLocaly();
            }
            else
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

                Canvaser.Instance.SBonuses.SetStartBonuses(LoginManager.User.Bonuses);
            }

            LoginManager.User.IceCream -= PriceValue;
            Canvaser.Instance.SetAllIceCreams(LoginManager.User.IceCream);
            FileExtensions.SaveJsonDataAsync(DataType.UserInfo, LoginManager.User);

            Canvaser.Instance.Shop.CheckBuyBtns();
            gameObject.SetActive(false);
        }

        public void BuyUpgradeLocaly()
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

            ShopManager.CurrentShop.BonusUpgrades.FirstOrDefault(bu => bu.Id == ItemId).Amount++;
            ShopManager.UpdateShopItems();
            GameController.Instance.LoadBonusesTime(LoginManager.User.BonusUpgrades);
        }
    }
}
