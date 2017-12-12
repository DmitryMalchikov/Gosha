using Newtonsoft.Json;
using System.Collections;
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
    }

    public void EnterPromoCodeAsync(string code)
    {
        InputString buy = new InputString() { Value = code };
        StartCoroutine(NetworkHelper.SendRequest(PromoCodeUrl, JsonConvert.SerializeObject(buy), "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Shop.PromoAnswer(response.Text);
            LoginManager.Instance.GetUserInfoAsync();
        }));
    }

    public void BuyItemAsync(int itemId, bool upgrade, int price, ResultCallback callback=null)
    {
        ItemBuyModel buy = new ItemBuyModel() { ItemId = itemId, Amount = 1 };
        StartCoroutine(NetworkHelper.SendRequest(BuyItemUrl, JsonConvert.SerializeObject(buy), "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            if (upgrade)
            {
                GetBonusesUpgradesAsync();
            }

            LoginManager.Instance.User.IceCream -= price;
            Canvaser.Instance.SetAllIceCreams(LoginManager.Instance.User.IceCream);
            Canvaser.Instance.Shop.CheckBuyBtns();

            LoginManager.Instance.GetUserInfoAsync();       
            
            if (callback != null)
            {
                callback();
            }
        }));
    }

    public void GetBonusesAsync(ResultCallback callback = null)
    {
        StartCoroutine(NetworkHelper.SendRequest(GetBonusesUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            List<ShopItem> upgrades = JsonConvert.DeserializeObject<List<ShopItem>>(response.Text);
            Canvaser.Instance.Shop.SetBonuses(upgrades);
            if (callback != null)
            {
                callback();
            }
        }));
    }
    public void GetBonusesUpgradesAsync(ResultCallback callback = null)
    {
        StartCoroutine(NetworkHelper.SendRequest(GetUpdgradesUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            List<ShopItem> upgrades = JsonConvert.DeserializeObject<List<ShopItem>>(response.Text);
            Canvaser.Instance.Shop.SetUpgrades(upgrades);
            if (callback != null)
            {
                callback();
            }
        }));
    }

    public void GetCardsAsync(ResultCallback callback = null)
    {
        StartCoroutine(NetworkHelper.SendRequest(GetCardsUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            List<ShopCard> upgrades = JsonConvert.DeserializeObject<List<ShopCard>>(response.Text);
            Canvaser.Instance.Shop.SetCards(upgrades);
            if (callback != null)
            {
                callback();
            }
        }));
    }
    public void GetCasesAsync(ResultCallback callback = null)
    {
        StartCoroutine(NetworkHelper.SendRequest(GetCasesUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            List<ShopItem> upgrades = JsonConvert.DeserializeObject<List<ShopItem>>(response.Text);
            Canvaser.Instance.Shop.SetCases(upgrades);
            if (callback != null)
            {
                callback();
            }
        }));
    }
    public void GetSuitAsync(int suitID)
    {
        InputInt input = new InputInt() { Value = suitID };
        StartCoroutine(NetworkHelper.SendRequest(GetSuitUrl, JsonConvert.SerializeObject(input), "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Suits.Open();
        }));
    }
}