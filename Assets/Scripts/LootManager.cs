using Newtonsoft.Json;
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
		StartCoroutine(NetworkHelper.SendRequest(OpenCaseUrl, new {CaseId = caseID, Language = (int)LocalizationManager.CurrentLanguage}, "application/json", (response) =>
        {
            Bonus bonus = JsonConvert.DeserializeObject<Bonus>(response.Text);
            Canvaser.Instance.CasesPanel.SetPrize(bonus);
        }));
    }

    public void GetDailyBonusAsync()
    {
        StartCoroutine(NetworkHelper.SendRequest(GetBonusUrl, null, "application/json", (response) =>
        {
            Debug.Log("OK");
            LoginManager.Instance.GetUserInfoAsync();
        }));
    }
}
