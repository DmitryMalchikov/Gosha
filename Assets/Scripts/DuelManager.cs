
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour
{


    public static DuelManager Instance { get; private set; }

    public string OfferDuelUrl = "/api/duels/offerduel";
    public string DuelRequestsUrl = "/api/duels/duelrequests";
    public string DuelOffersUrl = "/api/duels/dueloffers";
    public string AcceptDuelUrl = "/api/duels/acceptduel";
    public string DeclineDuelUrl = "/api/duels/declineduel";
    public string CancelDuelUrl = "/api/duels/cancelduel";
    public string SubmitDuelResultUrl = "/api/duels/submitduelresult";
    public string StartRunUrl = "/api/duels/startrun";
    public string DuelResultUrl = "/api/duels/duelresult";

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
        OfferDuelUrl = ServerInfo.GetUrl(OfferDuelUrl);
        DuelRequestsUrl = ServerInfo.GetUrl(DuelRequestsUrl);
        DuelOffersUrl = ServerInfo.GetUrl(DuelOffersUrl);
        AcceptDuelUrl = ServerInfo.GetUrl(AcceptDuelUrl);
        DeclineDuelUrl = ServerInfo.GetUrl(DeclineDuelUrl);
        CancelDuelUrl = ServerInfo.GetUrl(CancelDuelUrl);
        SubmitDuelResultUrl = ServerInfo.GetUrl(SubmitDuelResultUrl);
        StartRunUrl = ServerInfo.GetUrl(StartRunUrl);
        DuelResultUrl = ServerInfo.GetUrl(DuelResultUrl);
    }

    public void OfferDuelAsync(int userId, int bet)
    {
        DuelOfferModel input = new DuelOfferModel() { Id = userId, Bet = bet };

        StartCoroutine(NetworkHelper.SendRequest(OfferDuelUrl, JsonConvert.SerializeObject(input), "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.FriendsPanel.DuelOfferAnswer();
        }));
    }
    public void GetDuelsAsync(ResultCallback callback = null)
    {
        StartCoroutine(NetworkHelper.SendRequest(DuelOffersUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Duels.SetDuels(JsonConvert.DeserializeObject<List<DuelModel>>(response.Text));

            if (callback != null)
            {
                callback();
            }
        }));
    }
    public void GetDuelRequestsAsync(ResultCallback callback = null)
    {
        StartCoroutine(NetworkHelper.SendRequest(DuelRequestsUrl, "", "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Duels.SetRequests(JsonConvert.DeserializeObject<List<DuelModel>>(response.Text));

            if (callback != null)
            {
                callback();
            }
        }));
    }

    public void AcceptDuelAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        StartCoroutine(NetworkHelper.SendRequest(AcceptDuelUrl, JsonConvert.SerializeObject(input), "application/json", (response) =>
        {
            Debug.Log("OK");
        }));
    }

    public void DeclineDuelAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        StartCoroutine(NetworkHelper.SendRequest(DeclineDuelUrl, JsonConvert.SerializeObject(input), "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Duels.Open();
        }));
    }

    public void StartRunAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };
        StartCoroutine(NetworkHelper.SendRequest(StartRunUrl, JsonConvert.SerializeObject(input), "application/json", (response) =>
        {
            Debug.Log("OK");

        }));
    }

    public void GetDuelResultAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        StartCoroutine(NetworkHelper.SendRequest(DuelResultUrl, JsonConvert.SerializeObject(input), "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Duels.SetResult(JsonConvert.DeserializeObject<DuelResultModel>(response.Text));
        }));
    }

    public void SubmitDuelResultAsync(int duelID, int distance)
    {
        SubmitDuelScoreModel input = new SubmitDuelScoreModel() { Id = duelID, Distance = distance };

        StartCoroutine(NetworkHelper.SendRequest(SubmitDuelResultUrl, JsonConvert.SerializeObject(input), "application/json", (response) =>
        {
            DuelResultModel DRM = JsonConvert.DeserializeObject<DuelResultModel>(response.Text);
            if (DRM.FirstPlayer != null)
            {
                Canvaser.Instance.Duels.SetResult(DRM);
            }
        }));
    }
}
