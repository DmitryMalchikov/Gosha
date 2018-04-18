using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
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
        Canvaser.ShowLoading(true);
        StartCoroutine(NetworkHelper.SendRequest(GetTournamentLeadersUrl, null, "application/json", (response) =>
        {
            List<FriendModel> info = JsonConvert.DeserializeObject<List<FriendModel>>(response.Text);
            Canvaser.Instance.Tournament.SetTournamentTable(info);
            Canvaser.ShowLoading(false);
        }));
    }

    public void GetAllStatisticsAsync(int period, ResultCallback callback = null)
    {
        InputInt value = new InputInt() { Value = period };

        StartCoroutine(NetworkHelper.SendRequest(GetAllStatisticsUrl, value, "application/json", (response) =>
        {
            List<FriendModel> info = JsonConvert.DeserializeObject<List<FriendModel>>(response.Text);

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
        }));
    }

    public void GetTournamentInfoAsync()
    {
        Canvaser.ShowLoading(true);
        StartCoroutine(NetworkHelper.SendRequest(GetTournamentInfoUrl, new { Value = (int)LocalizationManager.CurrentLanguage }, "application/json", (response) =>
        {
            TournamentModel info = JsonConvert.DeserializeObject<TournamentModel>(response.Text);
            Canvaser.Instance.Tournament.SetTournamentInfo(info);

            Canvaser.ShowLoading(false);
            Canvaser.Instance.TasksPanel.SetActive(true);
        }));
    }
}
