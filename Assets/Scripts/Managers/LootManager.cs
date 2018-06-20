using UnityEngine;

public class LootManager : MonoBehaviour
{
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
        CoroutineManager.SendRequest(OpenCaseUrl, new { CaseId = caseID, Language = (int)LocalizationManager.CurrentLanguage }, (Bonus bonus) =>
         {
             Canvaser.Instance.CasesPanel.SetPrize(bonus);
         });
    }

    public void GetDailyBonusAsync()
    {
        CoroutineManager.SendRequest(GetBonusUrl, null, () =>
        {
            LoginManager.Instance.GetUserInfoAsync();
        });
    }
}
