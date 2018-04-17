using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelsPanel : MonoBehaviour {

    public Transform RequestsContent;
    public Transform DuelsContent;

    public GameObject DuelObject;
    public GameObject RequestObject;

    public List<DuelInfo> Requests = new List<DuelInfo>();
    public List<DuelInfo> Duels = new List<DuelInfo>();

    public DuelResult ResultPanel;

    public GameObject NoDuelsMsg;
    public GameObject NoRequestsMsg;

    public void Open()
    {
        Canvaser.ShowLoading(true);

        Synchroniser.NewSync(2);
        Synchroniser.OnActionsReady += () => gameObject.SetActive(true);
        Synchroniser.OnActionsReady += () => Canvaser.ShowLoading(false);

        ClearDuelPanel();
        DuelManager.Instance.GetDuelRequestsAsync(() => Synchroniser.SetReady(0));
        DuelManager.Instance.GetDuelsAsync(() => Synchroniser.SetReady(1));
    }

    public void SetDuels(List<DuelModel> duels)
    {
        NoDuelsMsg.SetActive(duels.Count == 0);
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
        infos.Clear();
        foreach (Transform item in content)
        {
            if (item.name != "Title")
            {
                Destroy(item.gameObject);
            }
        }
    }

    public void SetResult(DuelResultModel model)
    {
        ResultPanel.SetResult(model);
    }
}
