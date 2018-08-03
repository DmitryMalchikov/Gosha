using Newtonsoft.Json;
using UnityEngine;

public class ShopManager : Manager
{

    public string BuyItemUrl = "/api/shop/buyitem";
    public string GetBonusesUrl = "/api/shop/bonuses";
    public string GetCardsUrl = "/api/shop/cards";
    public string GetCasesUrl = "/api/shop/cases";
    public string GetShopItemsUrl = "/api/shop/shopitems";
    public string GetSuitUrl = "/api/shop/getsuit";
    public string GetUpdgradesUrl = "/api/shop/bonusupgrades";
    public string PromoCodeUrl = "/api/shop/promocode";
    public static ShopManager Instance { get; private set; }
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
           Debug.Log("OK");
           //show tasks
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
           Debug.Log("OK");
           Canvaser.Instance.Suits.OpenWithForceUpdate();
       });
    }

    private void Awake()
    {
        Instance = this;
    }

    //public void GetCardsAsync(ResultCallback callback = null)
    //{
    //    StartCoroutine(NetworkHelper.SendRequest(GetCardsUrl, null,  (response) =>
    //    {
    //        Debug.Log("OK");
    //        //show tasks
    //        List<ShopCard> upgrades = JsonConvert.DeserializeObject<List<ShopCard>>(response.Text);
    //        Canvaser.Instance.Shop.SetCards(upgrades);
    //        if (callback != null)
    //        {
    //            callback();
    //        }
    //    }));
    //}
    //public void GetCasesAsync(ResultCallback callback = null)
    //{
    //    StartCoroutine(NetworkHelper.SendRequest(GetCasesUrl, null,  (response) =>
    //    {
    //        Debug.Log("OK");
    //        //show tasks
    //        List<ShopItem> upgrades = JsonConvert.DeserializeObject<List<ShopItem>>(response.Text);
    //        Canvaser.Instance.Shop.SetCases(upgrades);
    //        if (callback != null)
    //        {
    //            callback();
    //        }
    //    }));
    //}
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

    private void SetUrls()
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
    private void Start()
    {
        SetUrls();
    }
    //public void GetBonusesAsync(ResultCallback callback = null)
    //{
    //    StartCoroutine(NetworkHelper.SendRequest(GetBonusesUrl, null,  (response) =>
    //    {
    //        Debug.Log("OK");
    //        //show tasks
    //        List<ShopItem> upgrades = JsonConvert.DeserializeObject<List<ShopItem>>(response.Text);
    //        Canvaser.Instance.Shop.SetBonuses(upgrades);
    //        if (callback != null)
    //        {
    //            callback();
    //        }
    //    }));
    //}
    //public void GetBonusesUpgradesAsync(ResultCallback callback = null)
    //{
    //    StartCoroutine(NetworkHelper.SendRequest(GetUpdgradesUrl, null,  (response) =>
    //    {
    //        Debug.Log("OK");
    //        //show tasks
    //        List<ShopItem> upgrades = JsonConvert.DeserializeObject<List<ShopItem>>(response.Text);
    //        Canvaser.Instance.Shop.SetUpgrades(upgrades);
    //        if (callback != null)
    //        {
    //            callback();
    //        }
    //    }));
    //}
}