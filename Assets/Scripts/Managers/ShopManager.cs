using Newtonsoft.Json;
using UnityEngine;

public class ShopManager : APIManager<ShopManager>
{
    public string BuyItemUrl = "/api/shop/buyitem";
    public string GetBonusesUrl = "/api/shop/bonuses";
    public string GetCardsUrl = "/api/shop/cards";
    public string GetCasesUrl = "/api/shop/cases";
    public string GetShopItemsUrl = "/api/shop/shopitems";
    public string GetSuitUrl = "/api/shop/getsuit";
    public string GetUpdgradesUrl = "/api/shop/bonusupgrades";
    public string PromoCodeUrl = "/api/shop/promocode";
    public static ShopModel CurrentShop;

    public void BuyItemAsync(int itemId, bool upgrade, int price, ResultCallback callback = null)
    {
        ItemBuyModel buy = new ItemBuyModel() { ItemId = itemId, Amount = 1 };
        CoroutineManager.SendRequest(BuyItemUrl, buy, () =>
       {
           if (upgrade)
           {
               string data = Extensions.LoadJsonData(DataType.Shop);
               ShopModel model = JsonConvert.DeserializeObject<ShopModel>(data);

               model.BonusUpgrades.Find(bu => bu.Id == itemId).Amount++;
               SetShopItems(model);
               ThreadHelper.RunNewThread(() =>
               {
                   Extensions.SaveJsonData(DataType.Shop, JsonConvert.SerializeObject(model));
               });
           }

           LoginManager.User.IceCream -= price;
           Canvaser.Instance.SetAllIceCreams(LoginManager.User.IceCream);
           Canvaser.Instance.Shop.CheckBuyBtns();

           LoginManager.Instance.GetUserInfoAsync();

           if (callback != null)
           {
               callback();
           }
       });
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

    public void GetShopItemsAsync(ResultCallback callback = null)
    {
        CoroutineManager.SendRequest(GetShopItemsUrl, null, (ShopModel model) =>
       {
           CurrentShop = model;
           GameController.SetHash("ShopHash", model.ShopHash);

           SetShopItems(model);

           if (callback != null)
           {
               callback();
           }
       }, type: DataType.Shop, loadingPanelsKey: "shop");
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
        ServerInfo.SetUrl(ref GetUpdgradesUrl);
        ServerInfo.SetUrl(ref PromoCodeUrl);
        ServerInfo.SetUrl(ref GetSuitUrl);
        ServerInfo.SetUrl(ref GetCardsUrl);
        ServerInfo.SetUrl(ref GetCasesUrl);
        ServerInfo.SetUrl(ref GetBonusesUrl);
        ServerInfo.SetUrl(ref GetShopItemsUrl);
    }
}