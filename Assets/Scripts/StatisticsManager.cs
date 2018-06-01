﻿using Newtonsoft.Json;
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
        StartCoroutine(NetworkHelper.SendRequest(GetTournamentLeadersUrl, null, "application/json", (response) =>
        {
            List<FriendOfferStatisticsModel> info = JsonConvert.DeserializeObject<List<FriendOfferStatisticsModel>>(response.Text);
            Canvaser.Instance.Tournament.SetTournamentTable(info);
        }));
    }

    public void OpenTournamentInfo()
    {
        Canvaser.Instance.Tournament.OpenTable();
    }

    public void GetAllStatisticsAsync(int period, List<GameObject> panels, ResultCallback callback = null)
    {
        InputInt value = new InputInt() { Value = period };

        Canvaser.AddLoadingPanel(panels, GetAllStatisticsUrl);
        Canvaser.ShowLoading(true, GetAllStatisticsUrl);

        StartCoroutine(NetworkHelper.SendRequest(GetAllStatisticsUrl, value, "application/json", (response) =>
        {
            List<FriendOfferStatisticsModel> info = JsonConvert.DeserializeObject<List<FriendOfferStatisticsModel>>(response.Text);

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
        }, loadingPanels: panels));
    }

    public void GetTournamentInfoAsync()
    {
        Canvaser.Instance.TasksPanel.SetActive(true);
        Canvaser.AddLoadingPanel(Canvaser.Instance.TasksPanel.LoadingPanels(), GetTournamentInfoUrl);

        StartCoroutine(NetworkHelper.SendRequest(GetTournamentInfoUrl, new { Value = (int)LocalizationManager.CurrentLanguage }, "application/json", (response) =>
        {
            TournamentModel info = JsonConvert.DeserializeObject<TournamentModel>(response.Text);
            Canvaser.Instance.Tournament.SetTournamentInfo(info);
            if (info.TournamentLeaders != null)
            {
                Canvaser.Instance.Tournament.SetTournamentTable(info.TournamentLeaders);
            }
        }));
    }
}
