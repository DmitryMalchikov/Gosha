public class LootManager : APIManager<LootManager>
{
    public string GetBonusUrl = "/api/loot/getbonus";
    public string OpenCaseUrl = "/api/loot/opencase";

    public override void SetUrls()
    {
        ServerInfo.SetUrl(ref GetBonusUrl);
        ServerInfo.SetUrl(ref OpenCaseUrl);
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
