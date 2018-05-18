using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour
{


    public static DuelManager Instance { get; private set; }

    public string FullDuelsInfo = "/api/duels/getallduels";
    public string OfferDuelUrl = "/api/duels/offerduel";
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
        FullDuelsInfo = ServerInfo.GetUrl(FullDuelsInfo);
        OfferDuelUrl = ServerInfo.GetUrl(OfferDuelUrl);
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

        StartCoroutine(NetworkHelper.SendRequest(OfferDuelUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.FriendsPanel.DuelOfferAnswer();
        },(response) =>
        {
            Canvaser.Instance.FriendsPanel.SetDuelWarning(response.Errors.Message);
        }));
    }
    public void GetDuelsAsync(ResultCallback callback = null)
    {
        StartCoroutine(NetworkHelper.SendRequest(FullDuelsInfo, null, "application/json", (response) =>
        {
            Debug.Log("OK");
            DuelsFullInfoModel model = JsonConvert.DeserializeObject<DuelsFullInfoModel>(response.Text);

            Canvaser.Instance.Duels.SetDuels(model.DuelOffers);
            Canvaser.Instance.Duels.SetRequests(model.DuelRequests);

            if (callback != null)
            {
                callback();
            }
        }));
    }

    public void AcceptDuelAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        StartCoroutine(NetworkHelper.SendRequest(AcceptDuelUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
        }));
    }

    public void DeclineDuelAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        StartCoroutine(NetworkHelper.SendRequest(DeclineDuelUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Duels.Open();
        }));
    }

    public void StartRunAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };
        StartCoroutine(NetworkHelper.SendRequest(StartRunUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");

        }));
    }

    public void GetDuelResultAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        StartCoroutine(NetworkHelper.SendRequest(DuelResultUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Duels.SetResult(JsonConvert.DeserializeObject<DuelResultModel>(response.Text));
        }));
    }

    public void SubmitDuelResultAsync(int duelID, int distance)
    {
        SubmitDuelScoreModel input = new SubmitDuelScoreModel() { Id = duelID, Distance = distance };

        StartCoroutine(NetworkHelper.SendRequest(SubmitDuelResultUrl, input, "application/json", (response) =>
        {
            DuelResultModel DRM = JsonConvert.DeserializeObject<DuelResultModel>(response.Text);
            if (DRM.FirstPlayer != null)
            {
                Canvaser.Instance.Duels.SetResult(DRM);
            }
        }));
    }
}
