using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public static InventoryManager Instance { get; private set; }
    
    public string GetSuitsUrl = "/api/inventory/costumes";
    public string GetCasesUrl = "/api/inventory/cases";
    public string GetCardsUrl = "/api/inventory/cards";
    public string GetBonusesUrl = "/api/inventory/bonuses";
    public string GetBonusUpgradesUrl = "/api/inventory/bonusupgrades";

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
        GetSuitsUrl = ServerInfo.GetUrl(GetSuitsUrl);
        GetCasesUrl = ServerInfo.GetUrl(GetCasesUrl);
        GetCardsUrl = ServerInfo.GetUrl(GetCardsUrl);
        GetBonusesUrl = ServerInfo.GetUrl(GetBonusesUrl);
        GetBonusUpgradesUrl = ServerInfo.GetUrl(GetBonusUpgradesUrl);
    }

    public void GetSuitsAsync()
    {
        StartCoroutine(NetworkHelper.SendRequest(GetSuitsUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            List<Costume> upgrades = JsonConvert.DeserializeObject<List<Costume>>(response.Text);
            Canvaser.Instance.Suits.SetCostumes(upgrades);
        }));
    }

    public void GetBonusesUpgradesAsync()
    {
        StartCoroutine(NetworkHelper.SendRequest(GetBonusUpgradesUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            List<InventoryItem> upgrades = JsonConvert.DeserializeObject<List<InventoryItem>>(response.Text);
            //Canvaser.Instance.Shop.SetUpgrades(upgrades);
        }));
    }
    public void GetMyBonusesAsync()
    {
        StartCoroutine(NetworkHelper.SendRequest(GetBonusesUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            List<InventoryItem> upgrades = JsonConvert.DeserializeObject<List<InventoryItem>>(response.Text);
            
        }));
    }

    public void GetMyCasesAsync()
    {
        //Canvaser.ShowLoading(true);
        StartCoroutine(NetworkHelper.SendRequest(GetCasesUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            List<InventoryItem> upgrades = JsonConvert.DeserializeObject<List<InventoryItem>>(response.Text);
            Canvaser.Instance.CasesPanel.SetCases(upgrades);
        }));
    }
}
