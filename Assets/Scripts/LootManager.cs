using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour {


    public static LootManager Instance { get; private set; }


    public string GetBonusUrl = "/api/loot/getbonus";
    public string OpenCaseUrl = "/api/loot/opencase";

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
        GetBonusUrl = ServerInfo.GetUrl(GetBonusUrl);
        OpenCaseUrl = ServerInfo.GetUrl(OpenCaseUrl);
    }
    public void OpenCaseAsync(int caseID)
    {
        //Canvaser.ShowLoading(true);
        InputInt input = new InputInt() { Value = caseID };
        StartCoroutine(NetworkHelper.SendRequest(OpenCaseUrl, JsonConvert.SerializeObject(input), "application/json", (response) =>
        {
            Debug.Log("OK");
            //show tasks
            Bonus bonus = JsonConvert.DeserializeObject<Bonus>(response.Text);
            InventoryManager.Instance.GetMyCasesAsync();
            //Canvaser.ShowLoading(false);
            Canvaser.Instance.CasesPanel.SetPrize(bonus);
            
        }));
    }

    public void GetDailyBonusAsync()
    {
        StartCoroutine(NetworkHelper.SendRequest(GetBonusUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            LoginManager.Instance.GetUserInfoAsync();
        }));
    }
}
