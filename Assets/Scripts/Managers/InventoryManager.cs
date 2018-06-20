using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Manager {

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

    public void GetSuitsAsync(bool forceUpdate = false)
    {
        CoroutineManager.SendRequest(GetSuitsUrl, null,  (SuitsModel upgrades) =>
        {
            Debug.Log("OK");
            GameController.SetHash("SuitsHash", upgrades.SuitsHash);         
            Canvaser.Instance.Suits.SetCostumes(upgrades.Costumes);
        }, 
        type: DataType.Suits, 
        forceUpdate: forceUpdate,
        loadingPanelsKey: "suits"
        ,preSuccessMethod:
        (response) => {
            Extensions.SaveJsonData(DataType.Suits, response.Text);
        });
    }

    //public void GetBonusesUpgradesAsync()
    //{
    //    StartCoroutine(NetworkHelper.SendRequest(GetBonusUpgradesUrl, null,  (response) =>
    //    {
    //        Debug.Log("OK");
    //        //show tasks
    //        List<InventoryItem> upgrades = JsonConvert.DeserializeObject<List<InventoryItem>>(response.Text);
    //        //Canvaser.Instance.Shop.SetUpgrades(upgrades);
    //    }));
    //}
    //public void GetMyBonusesAsync()
    //{
    //    StartCoroutine(NetworkHelper.SendRequest(GetBonusesUrl, null,  (response) =>
    //    {
    //        Debug.Log("OK");
    //        //show tasks
    //        List<InventoryItem> upgrades = JsonConvert.DeserializeObject<List<InventoryItem>>(response.Text);
            
    //    }));
    //}

    public void GetMyCasesAsync()
    {
        Canvaser.Instance.CasesPanel.SetCases(new List<InventoryItem> { new InventoryItem { Amount = LoginManager.Instance.User.Cases, Id = LoginManager.Instance.User.CaseId} });
        //Canvaser.ShowLoading(true);
        //StartCoroutine(NetworkHelper.SendRequest(GetCasesUrl, null,  (response) =>
        //{
        //    Debug.Log("OK");
        //    //show tasks
        //    List<InventoryItem> upgrades = JsonConvert.DeserializeObject<List<InventoryItem>>(response.Text);
        //    Canvaser.Instance.CasesPanel.SetCases(upgrades);
        //}));
    }
}
