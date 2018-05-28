using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelsPanel : MonoBehaviour {

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
        Canvaser.ShowLoading(true);

        ClearDuelPanel();
        DuelManager.Instance.GetDuelsAsync(() => gameObject.SetActive(true));
    }

    public void OpenDirectlyRequests()
    {
        Canvaser.ShowLoading(true);

        ClearDuelPanel();
        DuelManager.Instance.GetDuelsAsync(() => OpenRequests());
    }
    void OpenRequests()
    {
        gameObject.SetActive(true);
        RequestsToggle.isOn = true;
    }
    public void SetDuels(List<DuelModel> duels)
    {
        NoDuelsMsg.SetActive(duels.Count == 0);
        CurrentDuels.AddRange(duels);
        SetContent(DuelsContent, DuelObject, duels, Duels);        
    }

    public void SetRequests(List<DuelModel> requests)
    {
        NoRequestsMsg.SetActive(requests.Count == 0);
        gameObject.SetActive(true);
        SetContent(RequestsContent, RequestObject, requests, Requests);
    }

    public void SetContent(Transform content, GameObject obj,  List<DuelModel> models, List<DuelInfo> infos)
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

    public void SetResult(DuelResultModel model)
    {
        ResultPanel.SetResult(model);
    }
}
