using System.Linq;
using Assets.Scripts.DTO;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class ShopManager : APIManager<ShopManager>
    {
        public string BuyItemUrl = "/api/shop/buyitem";
        public string GetBonusesUrl = "/api/shop/bonuses";
        public string GetCardsUrl = "/api/shop/cards";
        public string GetCasesUrl = "/api/shop/cases";
        public string GetShopItemsUrl = "/api/shop/shopitems";
        public string GetSuitUrl = "/api/shop/getsuit";
        public string GetUpgradesUrl = "/api/shop/bonusupgrades";
        public string PromoCodeUrl = "/api/shop/promocode";
        public static ShopModel CurrentShop;

        public void BuyItemAsync(BuyInfo info, ResultCallback callback = null)
        {
            ItemBuyModel buy = new ItemBuyModel() { ItemId = info.ItemId, Amount = 1 };
            CoroutineManager.SendRequest(BuyItemUrl, buy, () =>
            {
                if (info.ItemName.Contains("Upgrade"))
                {
                    string data = FileExtensions.LoadJsonData(DataType.Shop);
                    ShopModel model = FileExtensions.TryParseData<ShopModel>(data);

                    model.BonusUpgrades.FirstOrDefault(bu => bu.Id == info.ItemId).Amount++;
                    SetShopItems(model);
                    FileExtensions.SaveJsonDataAsync(DataType.Shop, model);
                }

                LoginManager.User.IceCream -= info.PriceValue;
                Canvaser.Instance.SetAllIceCreams(LoginManager.User.IceCream);
                Canvaser.Instance.Shop.CheckBuyBtns();

                LoginManager.Instance.GetUserInfoAsync();
            }, errorMethod: (model) =>       
            {
                info.BuyItemLocaly();
            }, finallyMethod: () => callback());
        }

        public void EnterPromoCodeAsync(string code)
        {
            InputString buy = new InputString() { Value = code };
            CoroutineManager.SendRequest(PromoCodeUrl, buy, () =>
                {
                    Debug.Log("OK");
                    LoginManager.Instance.GetUserInfoAsync();
                }, preSuccessMethod:
                (response) =>
                {
                    Canvaser.Instance.Shop.PromoAnswer(response.Text);
                });
        }

        public CustomTask GetShopItemsAsync()
        {
            CustomTask task = new CustomTask();
            if (CurrentShop == null || LoginManager.User.ShopHash != CurrentShop.ShopHash)
            {
                CoroutineManager.SendRequest(GetShopItemsUrl, null, (ShopModel model) =>
                    {
                        CurrentShop = model;
                        HashManager.SetShopHash(model.ShopHash);
                        SetShopItems(model);

                        task.Ready = true;
                    }, 
                    type: DataType.Shop, loadingPanelsKey: "shop");
            }
            else
            {
                SetShopItems(CurrentShop);
            }
            return task;
        }

        public void GetSuitAsync(int suitID)
        {
            InputInt input = new InputInt() { Value = suitID };
            CoroutineManager.SendRequest(GetSuitUrl, input, () =>
            {
                Canvaser.Instance.Suits.OpenWithForceUpdate();
            });
        }

        private void SetShopItems(ShopModel model)
        {
            Canvaser.Instance.Shop.SetBonuses(model.Bonuses);
            Canvaser.Instance.Shop.SetCases(model.Cases);
            Canvaser.Instance.Shop.SetCards(model.Cards);
            Canvaser.Instance.Shop.SetUpgrades(model.BonusUpgrades);
            Canvaser.Instance.Shop.gameObject.SetActive(true);
        }

        public static void UpdateShopItems()
        {
            Instance.SetShopItems(CurrentShop);
        }

        public override void SetUrls()
        {
            ServerInfo.SetUrl(ref BuyItemUrl);
            ServerInfo.SetUrl(ref GetUpgradesUrl);
            ServerInfo.SetUrl(ref PromoCodeUrl);
            ServerInfo.SetUrl(ref GetSuitUrl);
            ServerInfo.SetUrl(ref GetCardsUrl);
            ServerInfo.SetUrl(ref GetCasesUrl);
            ServerInfo.SetUrl(ref GetBonusesUrl);
            ServerInfo.SetUrl(ref GetShopItemsUrl);
        }
    }
}