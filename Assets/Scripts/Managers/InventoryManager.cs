using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : APIManager<InventoryManager>
{
    public string GetSuitsUrl = "/api/inventory/costumes";
    public string GetCasesUrl = "/api/inventory/cases";
    public string GetCardsUrl = "/api/inventory/cards";
    public string GetBonusesUrl = "/api/inventory/bonuses";
    public string GetBonusUpgradesUrl = "/api/inventory/bonusupgrades";

    public override void SetUrls()
    {
        ServerInfo.SetUrl(ref GetSuitsUrl);
        ServerInfo.SetUrl(ref GetCasesUrl);
        ServerInfo.SetUrl(ref GetCardsUrl);
        ServerInfo.SetUrl(ref GetBonusesUrl);
        ServerInfo.SetUrl(ref GetBonusUpgradesUrl);
    }

    public void GetSuitsAsync(bool forceUpdate = false)
    {
        CoroutineManager.SendRequest(GetSuitsUrl, null, (SuitsModel upgrades) =>
       {
           HashManager.SetSuitsHash(upgrades.SuitsHash);
           Canvaser.Instance.Suits.SetCostumes(upgrades.Costumes);
       },
        type: DataType.Suits,
        forceUpdate: forceUpdate,
        loadingPanelsKey: "suits");
    }

    public void GetMyCasesAsync()
    {
        if (Canvaser.Instance.IsLoggedIn())
        {
            Canvaser.Instance.CasesPanel.SetCases(new List<InventoryItem> { new InventoryItem { Amount = LoginManager.User.Cases, Id = LoginManager.User.CaseId } });
        }
    }
}
