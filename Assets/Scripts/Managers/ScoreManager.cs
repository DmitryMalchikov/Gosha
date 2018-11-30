using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : APIManager<ScoreManager>
{
    private static Dictionary<int, byte> uses;

    public string SubmitScoreUrl = "/api/user/submitscore";
    public string UseBonusUrl = "/api/gameplay/usebonus";
    public string GetKeyUrl = "";
    public string ContinueForMoneyUrl = "/api/gameplay/continueformoney";

    public static void StartRun()
    {
        uses = new Dictionary<int, byte>();
    }

    public override void SetUrls()
    {
        ServerInfo.SetUrl(ref SubmitScoreUrl);
        ServerInfo.SetUrl(ref UseBonusUrl);
        ServerInfo.SetUrl(ref GetKeyUrl);
        ServerInfo.SetUrl(ref ContinueForMoneyUrl);
    }

    bool SubmittingScore = false;

    public void SubmitScoreAsync(int distance, int iceCream, int boxes)
    {
        if (!LoginManager.LocalUser)
        {
            StartCoroutine(SubmitScoreNetowrk(distance, iceCream, boxes));
        }
        else
        {
            SubmitScoreLocal(distance, iceCream, boxes);
        }
    }

    System.Collections.IEnumerator SubmitScoreNetowrk(int distance, int iceCream, int boxes)
    {
        yield return new WaitUntil(() => !SubmittingScore);

        SubmittingScore = true;

        Debug.Log(iceCream);
        string key = "";
        yield return CoroutineManager.SendRequest(GetKeyUrl, null,
                (string response) =>
                {
                    key = response.Replace("\"", string.Empty);
                });

        Debug.Log(key);
        key = Utils.CalculateMD5Hash(key);
        SubmitScoreModel model = new SubmitScoreModel()
        {
            Distance = distance,
            IceCreamCount = iceCream,
            Uses = uses,
            Key = key,
            CasesCount = boxes,
            NotContinued = !GameController.Instance.Continued
        };

        CoroutineManager.SendRequest(SubmitScoreUrl, model,
        () =>
        {
            if (GameController.Instance.Continued)
            {
                GameController.Instance.Continued = false;
            }
            LoginManager.Instance.GetUserInfoAsync();
            SubmittingScore = false;
        });
    }

    private void SubmitScoreLocal(int distance, int iceCream, int boxes)
    {
        LoginManager.User.IceCream += iceCream;
        LoginManager.User.Cases += boxes;

        Canvaser.Instance.SetAllIceCreams(LoginManager.User.IceCream);
        Canvaser.Instance.SetNotifications(LoginManager.User);

        FileExtensions.SaveJsonDataAsync(DataType.UserInfo, LoginManager.User);
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
            CoroutineManager.SendRequest(UseBonusUrl, value,
            errorMethod: (response) =>
            {
                SetUses(bonusInvId);
            });
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
}
