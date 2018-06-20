using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : Manager
{

    public static StatisticsManager Instance { get; private set; }

    public string GetTournamentLeadersUrl = "/api/statistics/tournamentleaders";
    public string GetAllStatisticsUrl = "/api/statistics/allstatistics";
    public string GetTournamentInfoUrl = "/api/statistics/tournamentinfo";


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
        GetTournamentLeadersUrl = ServerInfo.GetUrl(GetTournamentLeadersUrl);
        GetAllStatisticsUrl = ServerInfo.GetUrl(GetAllStatisticsUrl);
        GetTournamentInfoUrl = ServerInfo.GetUrl(GetTournamentInfoUrl);
    }
    public void GetTournamentLeadersAsync()
    {
        CoroutineManager.SendRequest(GetTournamentLeadersUrl, null,  (List<FriendOfferStatisticsModel> info) =>
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

        CoroutineManager.SendRequest(GetAllStatisticsUrl, value,  (List<FriendOfferStatisticsModel> info) =>
        {
            if (period == 2)
            {
                Canvaser.Instance.Stats.SetAllTimeLeaders(info);
            }
            else
            {
                Canvaser.Instance.Stats.SetLeaders(info);
            }

            if (callback != null)
            {
                callback();
            }
        }, loadingPanelsKey: loadingPanelsKey);
    }

    public void GetTournamentInfoAsync()
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
