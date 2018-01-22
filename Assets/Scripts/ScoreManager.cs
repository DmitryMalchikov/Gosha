using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class ScoreManager : MonoBehaviour
{
    private static Dictionary<int, byte> uses;

    public static ScoreManager Instance { get; private set; }

    public string SubmitScoreUrl = "/api/user/submitscore";
    public string UseBonusUrl = "/api/gameplay/usebonus";
    public string GetKeyUrl = "";
    public string ContinueForMoneyUrl = "/api/gameplay/continueformoney";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetUrls();
    }

    public static void StartRun()
    {
        uses = new Dictionary<int, byte>();
    }

    public void SetUrls()
    {
        SubmitScoreUrl = ServerInfo.GetUrl(SubmitScoreUrl);
        UseBonusUrl = ServerInfo.GetUrl(UseBonusUrl);
        GetKeyUrl = ServerInfo.GetUrl(GetKeyUrl);
        ContinueForMoneyUrl = ServerInfo.GetUrl(ContinueForMoneyUrl);
    }

    public void SubmitScoreAsync(int distance, int iceCream, int boxes)
    {
        StartCoroutine(SubmitScore(distance, iceCream, boxes));
    }

    System.Collections.IEnumerator SubmitScore(int distance, int iceCream, int boxes)
    {
        
        string key = "";
        yield return StartCoroutine(NetworkHelper.SendRequest(GetKeyUrl, "", "application/json",
                (response) =>
                {
                    key = response.Text.Replace("\"", string.Empty);
                }
                ));

        Debug.Log(key);
        key = Utils.CalculateMD5Hash(key);
        SubmitScoreModel model = new SubmitScoreModel() { Distance = distance, IceCreamCount = iceCream, Uses = uses, Key = key, CasesCount = boxes };
        StartCoroutine(NetworkHelper.SendRequest(SubmitScoreUrl, JsonConvert.SerializeObject(model), "application/json",
        (response) =>
        {
            LoginManager.Instance.GetUserInfoAsync();
        }
        ));
    }

    public void UseBonusAsync(int bonusInvId)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            SetUses(bonusInvId);
        }
        else
        {
            InputInt value = new InputInt() { Value = bonusInvId };
            StartCoroutine(NetworkHelper.SendRequest(UseBonusUrl, JsonConvert.SerializeObject(value), "application/json",
            (response) =>
            {
            },
            (response) =>
            {
                SetUses(bonusInvId);
            }
            ));
        }
    }

    private void SetUses(int bonusInvId)
    {
        if (!uses.ContainsKey(bonusInvId))
        {
            uses.Add(bonusInvId, 1);
        }
        else
        {
            uses[bonusInvId] += 1;
        }
    }

    public void ContinueForMoney()
    {
        StartCoroutine(ContinueForMoneyAsync());
    }

    System.Collections.IEnumerator ContinueForMoneyAsync()
    {
        yield return StartCoroutine(NetworkHelper.SendRequest(ContinueForMoneyUrl, "", "application/json",
            (response) => 
            {
                ContinuePanel.Instance.gameObject.SetActive(false);
                GameController.Instance.ContinueGameForMoney();                
            },
            (response) =>
            {
                //TODO: add warning (no money, error...)
            }
            ));
    }
}
