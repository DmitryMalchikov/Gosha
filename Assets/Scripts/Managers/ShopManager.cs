using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    public static ShopManager Instance { get; private set; }

    public string BuyItemUrl = "/api/shop/buyitem";
    public string GetUpdgradesUrl = "/api/shop/bonusupgrades";
    public string PromoCodeUrl = "/api/shop/promocode";
    public string GetSuitUrl = "/api/shop/getsuit";
    public string GetCardsUrl = "/api/shop/cards";
    public string GetCasesUrl = "/api/shop/cases";
	public string GetBonusesUrl = "/api/shop/bonuses";
	public string GetShopItemsUrl = "/api/shop/shopitems";


	private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetUrls();
    }
    public void SetUrls()
    {
        BuyItemUrl = ServerInfo.GetUrl(BuyItemUrl);
        GetUpdgradesUrl = ServerInfo.GetUrl(GetUpdgradesUrl);
        PromoCodeUrl = ServerInfo.GetUrl(PromoCodeUrl);
        GetSuitUrl = ServerInfo.GetUrl(GetSuitUrl);
        GetCardsUrl = ServerInfo.GetUrl(GetCardsUrl);
        GetCasesUrl = ServerInfo.GetUrl(GetCasesUrl);
        GetBonusesUrl = ServerInfo.GetUrl(GetBonusesUrl);
		GetShopItemsUrl = ServerInfo.GetUrl(GetShopItemsUrl);
    }

    public void EnterPromoCodeAsync(string code)
    {
        InputString buy = new InputString() { Value = code };
        CoroutineManager.SendRequest(PromoCodeUrl, buy,  () =>
        {
            Debug.Log("OK");            
            LoginManager.Instance.GetUserInfoAsync();
        }, preSuccessMethod:
        (response) => {
            Canvaser.Instance.Shop.PromoAnswer(response.Text);
        });
    }

    public void BuyItemAsync(int itemId, bool upgrade, int price, ResultCallback callback=null)
    {
        ItemBuyModel buy = new ItemBuyModel() { ItemId = itemId, Amount = 1 };
        CoroutineManager.SendRequest(BuyItemUrl, buy,  () =>
        {
            Debug.Log("OK");
            //show tasks
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

            LoginManager.Instance.User.IceCream -= price;
            Canvaser.Instance.SetAllIceCreams(LoginManager.Instance.User.IceCream);
            Canvaser.Instance.Shop.CheckBuyBtns();

            LoginManager.Instance.GetUserInfoAsync();       
            
            if (callback != null)
            {
                callback();
            }
        });
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

    public void GetSuitAsync(int suitID)
    {
        InputInt input = new InputInt() { Value = suitID };
        CoroutineManager.SendRequest(GetSuitUrl, input,  () =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Suits.Open();
        });
    }

    public void GetShopItemsAsync(List<GameObject> panels, ResultCallback callback = null)
	{
        Canvaser.AddLoadingPanel(panels, GetShopItemsUrl);
        Canvaser.ShowLoading(true, GetShopItemsUrl);

        CoroutineManager.SendRequest(GetShopItemsUrl, null,  (ShopModel model) =>
		{
			Debug.Log("OK");
			//show tasks
            GameController.SetHash("ShopHash", model.ShopHash);            

            SetShopItems(model);

			if (callback != null)
			{
				callback();
			}
		}, type: DataType.Shop
        ,preSuccessMethod:
        (response) => {
            Extensions.SaveJsonData(DataType.Shop, response.Text);
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
}