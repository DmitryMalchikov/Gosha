using System.Collections.Generic;

public class StatisticsManager : APIManager<StatisticsManager>
{
    public string GetTournamentLeadersUrl = "/api/statistics/tournamentleaders";
    public string GetAllStatisticsUrl = "/api/statistics/allstatistics";
    public string GetTournamentInfoUrl = "/api/statistics/tournamentinfo";

    public override void SetUrls()
    {
        ServerInfo.SetUrl(ref GetTournamentLeadersUrl);
        ServerInfo.SetUrl(ref GetAllStatisticsUrl);
        ServerInfo.SetUrl(ref GetTournamentInfoUrl);
    }
    public void GetTournamentLeadersAsync()
    {
        CoroutineManager.SendRequest(GetTournamentLeadersUrl, null, (FriendOfferStatisticsModel[] info) =>
       {
           Canvaser.Instance.Tournament.SetTournamentTable(info);
       });
    }

    public void OpenTournamentInfo()
    {
        Canvaser.Instance.Tournament.OpenTable();
    }

    public void GetAllStatisticsAsync(int period, ResultCallback callback = null)
    {
        InputInt value = new InputInt() { Value = period };
        var loadingPanelsKey = period == 2 ? "alltimeleaders" : "leaders";

        CoroutineManager.SendRequest(GetAllStatisticsUrl, value, (List<FriendOfferStatisticsModel> info) =>
       {
           if (period == 2)
           {
               Canvaser.Instance.Stats.SetAllTimeLeaders(info);
           }
           else
           {
               Canvaser.Instance.Stats.SetLeaders(info);
               Canvaser.Instance.Stats.LeadersText.text = period == 0 ? LocalizationManager.GetLocalizedValue("weekleaders") : LocalizationManager.GetLocalizedValue("monthleaders");
           }

           if (callback != null)
           {
               callback();
           }
       }, loadingPanelsKey: loadingPanelsKey);
    }

    public void GetTournamentInfoAsync()
    {

        if (Canvaser.Instance.IsLoggedIn())
        {
            Canvaser.Instance.TasksPanel.SetActive(true);

            CoroutineManager.SendRequest(GetTournamentInfoUrl, new { Value = (int)LocalizationManager.CurrentLanguage },
            (TournamentModel info) =>
            {
                Canvaser.Instance.Tournament.SetTournamentInfo(info);
                if (info.TournamentLeaders != null)
                {
                    Canvaser.Instance.Tournament.SetTournamentTable(info.TournamentLeaders);
                }
            }, loadingPanelsKey: "tournament");
        }
    }
}
