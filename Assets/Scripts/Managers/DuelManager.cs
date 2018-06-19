using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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

        CoroutineManager.SendRequest(OfferDuelUrl, input,  () =>
        {
            Debug.Log("OK");
            Canvaser.Instance.FriendsPanel.DuelOfferAnswer();
        }, (response) =>
         {
             Canvaser.Instance.FriendsPanel.SetDuelWarning(response.Errors.First().Value[0]);
         });
    }
    public void GetDuelsAsync(List<GameObject> panels, ResultCallback callback = null)
    {
        Canvaser.AddLoadingPanel(panels, FullDuelsInfo);
        Canvaser.ShowLoading(true, FullDuelsInfo);

        CoroutineManager.SendRequest(FullDuelsInfo, null,  (DuelsFullInfoModel model) =>
        {
            GameController.SetHash("DuelsHash", model.DuelsHash);
            Canvaser.Instance.Duels.SetDuels(model.DuelOffers);
            Canvaser.Instance.Duels.SetRequests(model.DuelRequests);

            if (callback != null)
            {
                callback();
            }
        }, type: DataType.Duels,
        preSuccessMethod: (response) =>
        {
            Extensions.SaveJsonData(DataType.Duels, response.Text);
        });
    }

    public void AcceptDuelAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        CoroutineManager.SendRequest(AcceptDuelUrl, input,  () =>
        {
            Debug.Log("OK");
        });
    }

    public void DeclineDuelAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        CoroutineManager.SendRequest(DeclineDuelUrl, input,  () =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Duels.Open();
        });
    }

    public void StartRunAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };
        CoroutineManager.SendRequest(StartRunUrl, input,  () =>
        {
            Debug.Log("OK");
        });
    }

    public void GetDuelResultAsync(int duelID)
    {
        InputInt input = new InputInt() { Value = duelID };

        CoroutineManager.SendRequest(DuelResultUrl, input,  (DuelResultModel response) =>
        {
            Debug.Log("OK");
            Canvaser.Instance.Duels.SetResult(response);
        });
    }

    public void SubmitDuelResultAsync(int duelID, int distance)
    {
        SubmitDuelScoreModel input = new SubmitDuelScoreModel() { Id = duelID, Distance = distance };

        CoroutineManager.SendRequest(SubmitDuelResultUrl, input,  (DuelResultModel DRM) =>
        {
            //DuelResultModel DRM = JsonConvert.DeserializeObject<DuelResultModel>(response.Text);
            if (DRM.FirstPlayer != null)
            {
                Canvaser.Instance.Duels.SetResult(DRM);
            }
        });
    }
}
