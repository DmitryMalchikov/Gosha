using System.Collections.Generic;
using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DuelsPanel : MonoBehaviour
    {
        public Transform RequestsContent;
        public Transform DuelsContent;

        public GameObject DuelObject;
        public GameObject RequestObject;

        public List<DuelInfo> Requests = new List<DuelInfo>();
        public List<DuelInfo> Duels = new List<DuelInfo>();
        public List<DuelModel> CurrentDuels = new List<DuelModel>();

        public DuelResult ResultPanel;

        public GameObject NoDuelsMsg;
        public GameObject NoRequestsMsg;

        public Toggle DuelsToggle;
        public Toggle RequestsToggle;

        public void Open()
        {
            if (Canvaser.Instance.IsLoggedIn())
            {
                ClearDuelPanel();
                gameObject.SetActive(true);
                DuelManager.Instance.GetDuelsAsync();
            }
        }

        public void OpenDirectlyRequests()
        {
            ClearDuelPanel();
            gameObject.SetActive(true);
            DuelManager.Instance.GetDuelsAsync(OpenRequests);
        }

        private void OpenRequests()
        {
            gameObject.SetActive(true);
            RequestsToggle.isOn = true;
        }
        public void SetDuels(DuelModel[] duels)
        {
            NoDuelsMsg.SetActive(duels.Length == 0);
            CurrentDuels.AddRange(duels);
            SetContent(DuelsContent, DuelObject, duels, Duels);
        }

        public void SetRequests(DuelModel[] requests)
        {
            NoRequestsMsg.SetActive(requests.Length == 0);
            SetContent(RequestsContent, RequestObject, requests, Requests);
        }

        public void SetContent(Transform content, GameObject obj, DuelModel[] models, List<DuelInfo> infos)
        {
            foreach (DuelModel item in models)
            {
                DuelInfo newInfo = Instantiate(obj, content).GetComponent<DuelInfo>();
                newInfo.SetDuelPanel(item);
                infos.Add(newInfo);
            }
        }

        public void ClearDuelPanel()
        {
            ClearContent(DuelsContent, Duels);
            ClearContent(RequestsContent, Requests);
        }

        public void ClearContent(Transform content, List<DuelInfo> infos)
        {
            CurrentDuels.Clear();
            infos.Clear();
            foreach (Transform item in content)
            {
                if (item.name != "Title")
                {
                    Destroy(item.gameObject);
                }
            }
        }

        public void UpdatePanel(DuelModel duel)
        {
            CurrentDuels.Remove(duel);
            NoDuelsMsg.SetActive(CurrentDuels.Count == 0);
        }

        public void SetResult(DuelRes model)
        {
            ResultPanel.SetResult(model);
        }
    }
}
