﻿using System.Linq;
using Assets.Scripts.DTO;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class DuelManager : APIManager<DuelManager>
    {
        private static int _duelId;

        public static bool InDuel { get; private set; }

        public string FullDuelsInfo = "/api/duels/getallduels";
        public string OfferDuelUrl = "/api/duels/offerduel";
        public string AcceptDuelUrl = "/api/duels/acceptduel";
        public string DeclineDuelUrl = "/api/duels/declineduel";
        public string CancelDuelUrl = "/api/duels/cancelduel";
        public string SubmitDuelResultUrl = "/api/duels/submitduelresult";
        public string StartRunUrl = "/api/duels/startrun";
        public string DuelResultUrl = "/api/duels/duelresult";

        public override void SetUrls()
        {
            ServerInfo.SetUrl(ref FullDuelsInfo);
            ServerInfo.SetUrl(ref OfferDuelUrl);
            ServerInfo.SetUrl(ref AcceptDuelUrl);
            ServerInfo.SetUrl(ref DeclineDuelUrl);
            ServerInfo.SetUrl(ref CancelDuelUrl);
            ServerInfo.SetUrl(ref SubmitDuelResultUrl);
            ServerInfo.SetUrl(ref StartRunUrl);
            ServerInfo.SetUrl(ref DuelResultUrl);
        }

        public void OfferDuelAsync(int userId, int bet)
        {
            DuelOfferModel input = new DuelOfferModel() { Id = userId, Bet = bet };

            CoroutineManager.SendRequest(OfferDuelUrl, input, () =>
            {
                Debug.Log("OK");
                Canvaser.Instance.FriendsPanel.DuelOfferAnswer();
            }, (response) =>
            {
                Canvaser.Instance.FriendsPanel.SetDuelWarning(response.Errors.First().Value[0]);
            });
        }
        public void GetDuelsAsync(ResultCallback callback = null)
        {
            CoroutineManager.SendRequest(FullDuelsInfo, null, (DuelsFullInfoModel model) =>
            {
                HashManager.SetDuelsHash(model.DuelsHash);
                Canvaser.Instance.Duels.SetDuels(model.DuelOffers);
                Canvaser.Instance.Duels.SetRequests(model.DuelRequests);

                if (callback != null)
                {
                    callback();
                }
            }, type: DataType.Duels, loadingPanelsKey: "duels");
        }

        public void AcceptDuelAsync(int duelId)
        {
            InputInt input = new InputInt() { Value = duelId };

            CoroutineManager.SendRequest(AcceptDuelUrl, input, () =>
            {
            });
        }

        public void DeclineDuelAsync(int duelId)
        {
            InputInt input = new InputInt() { Value = duelId };

            CoroutineManager.SendRequest(DeclineDuelUrl, input, () =>
            {
                Canvaser.Instance.Duels.Open();
            });
        }

        public void StartRunAsync(int duelId)
        {
            SetDuel(duelId);
            InputInt input = new InputInt() { Value = duelId };
            CoroutineManager.SendRequest(StartRunUrl, input, () =>
            {
            });
        }

        public void GetDuelResultAsync(int duelId)
        {
            InputInt input = new InputInt() { Value = duelId };

            CoroutineManager.SendRequest(DuelResultUrl, input, (DuelResultModel response) =>
            {
                Canvaser.Instance.Duels.SetResult(new DuelRes(response));
            });
        }

        public void SubmitDuelResultAsync(int distance)
        {
            if (!InDuel)
            {
                return;
            }

            InDuel = false;
            SubmitDuelScoreModel input = new SubmitDuelScoreModel() { Id = _duelId, Distance = distance };

            CoroutineManager.SendRequest(SubmitDuelResultUrl, input, (DuelResultModel DRM) =>
            {
                if (DRM.FirstPlayer != null)
                {
                    Canvaser.Instance.Duels.SetResult(new DuelRes(DRM));
                }
            });
        }

        public static void SetDuel(int duelId)
        {
            InDuel = true;
            _duelId = duelId;
        }
    }
}
