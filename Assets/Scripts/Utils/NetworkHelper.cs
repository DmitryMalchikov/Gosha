using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using uTasks;

public static class NetworkHelper
{
    public static AnswerModel GetResponsePost(string url, string postParameters, string ContentType, List<Header> headers = null)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        var postData = postParameters;
        var data = Encoding.UTF8.GetBytes(postData);

        if (!string.IsNullOrEmpty(ContentType))
        {
            request.ContentType = ContentType;
        }

        request.ContentLength = data.Length;
        request.Method = "POST";
        request.PreAuthenticate = true;
        if (headers != null)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                request.Headers[headers[i].Name] = headers[i].Value;
            }
        }
        request.UserAgent = "GoshaGame";
        //request.Proxy = new WebProxy("http://127.0.0.1:8888");

        using (var stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        try
        {
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return new AnswerModel(responseFromServer);
        }
        catch (WebException e)
        {
            return HandleWebException(e);
        }
    }

    public static IEnumerator SendRequest(string url, object parameters, string contentType, Action successMethod = null, Action<AnswerModel> errorMethod = null, string loadingPanelsKey = null, DataType type = DataType.Network, bool forceUpdate = false, Action<AnswerModel> preSuccessMethod = null, Action finallyMethod = null)
    {
        AnswerModel response = new AnswerModel();
        for (IEnumerator e = StartRequest(url, parameters, contentType, forceUpdate, type, response, loadingPanelsKey); e.MoveNext();)
        {
            yield return e.Current;
        }

        try
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (preSuccessMethod != null)
                {
                    preSuccessMethod(response);
                }
                if (successMethod != null)
                {
                    successMethod();
                }
            }
            else
            {
                HandleRequestError(response, errorMethod);
            }
        }
        finally
        {
            if (finallyMethod != null)
            {
                finallyMethod();
            }
            Extensions.ShowGameObjects(LoadingManager.GetPanelsByKey(loadingPanelsKey), false);
        }
    }

    public static IEnumerator SendRequest<T>(string url, object parameters, string contentType, Action<T> successMethod, Action<AnswerModel> errorMethod = null, string loadingPanelsKey = null, DataType type = DataType.Network, bool forceUpdate = false, Action<AnswerModel> preSuccessMethod = null, Action finallyMethod = null)
    {       
        AnswerModel response = new AnswerModel();
        for (IEnumerator e = StartRequest(url, parameters, contentType, forceUpdate, type, response, loadingPanelsKey); e.MoveNext();)
        {
            yield return e.Current;
        }

        try
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (preSuccessMethod != null)
                {
                    preSuccessMethod(response);
                }

                T data = default(T);
                bool converted = false;

                Task.Run(() => data = JsonConvert.DeserializeObject<T>(response.Text)).Then(() => converted = true);

                while (!converted)
                {
                    yield return null;
                }

                successMethod(data);
            }
            else
            {
                HandleRequestError(response, errorMethod);
            }
        }
        finally
        {
            if (finallyMethod != null)
            {
                finallyMethod();
            }
            Extensions.ShowGameObjects(LoadingManager.GetPanelsByKey(loadingPanelsKey), false);
        }
    }

    public static IEnumerator StartRequest(string url, object parameters, string contentType, bool forceUpdate, DataType type, AnswerModel response, string loadingPanelsKey)
    {
        if (type != DataType.Network && forceUpdate == false)
        {
            forceUpdate = GetForceUpdate(type);
        }
        if (forceUpdate)
        {
            Extensions.ShowGameObjects(LoadingManager.GetPanelsByKey(loadingPanelsKey));
        }

        while (GameController.PersistentDataPath == null)
        {
            yield return null;
        }

        ThreadHelper.RunNewThread(() =>
        {
            string parms = string.Empty;
            if (parameters != null)
            {
                if (!(parameters is string))
                {
                    parms = JsonConvert.SerializeObject(parameters);
                }
                else
                {
                    parms = parameters as string;
                }
            }

            //load local data
            //forceUpdate = GetForceUpdate(type);
            if (!forceUpdate)
            {
                string data = Extensions.LoadJsonData(type);
                if (!string.IsNullOrEmpty(data) || type == DataType.UserInfo)
                {
                    response.SetFields(new AnswerModel(data));
                }
            }
            if (response.StatusCode == 0)
            {
                LoadingManager.PanelKeyToEnable = loadingPanelsKey;
                response.SetFields(GetResponsePost(url, parms, contentType, LoginManager.Instance.Headers));
            }
        });

        while (response.StatusCode == 0)
        {
            yield return null;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.OK && forceUpdate)
        {
            if (type != DataType.Network && type != DataType.UserInfo)
            {
                ThreadHelper.RunNewThread(() =>
                {
                    Extensions.SaveJsonData(type, response.Text);
                });
            }
        }
        }

    public static void HandleRequestError(AnswerModel response, Action<AnswerModel> errorMethod)
    {
        Debug.Log("Error");
        if (response.Errors != null)
        {
            Debug.Log(response.Errors);
        }
        if (errorMethod != null)
        {
            errorMethod(response);
        }
    }

    private static AnswerModel HandleWebException(WebException exception)
    {
        using (WebResponse response = exception.Response)
        {
            HttpWebResponse httpResponse = (HttpWebResponse)response;
            using (Stream data1 = response.GetResponseStream())
            {
                string text = new StreamReader(data1).ReadToEnd();
                Debug.Log(text);
                var errors = new ErrorAnswer();
                Dictionary<string, IList<string>> errorCodes = null;
                try
                {
                    errors = JsonConvert.DeserializeObject<ErrorAnswer>(text);
                    if (errors.ModelState != null && errors.ModelState.Any())
                    {
                        errorCodes = errors.ModelState;
                    }
                    else
                    {
                        errorCodes = new Dictionary<string, IList<string>> { { "Message", new List<string> { errors.Message } } };
                    }
                }
                catch
                {
                    errorCodes = new Dictionary<string, IList<string>> { { "Message", new List<string> { "FatalError" } } };
                }
                return new AnswerModel() { StatusCode = httpResponse.StatusCode, Errors = errorCodes };
            }
        }
    }

    private static bool GetForceUpdate(DataType type)
    {
        string savedHash;

        if (LoginManager.LocalUser)
        {
            return false;
        }

        switch (type)
        {
            case DataType.Duels:
                savedHash = GameController.DuelsHash;
                return LoginManager.User.DuelsHash != savedHash;
            case DataType.Friends:
                savedHash = GameController.FriendsHash;
                return LoginManager.User.FriendsHash != savedHash;
            case DataType.Shop:
                savedHash = GameController.ShopHash;
                return LoginManager.User.ShopHash != savedHash;
            case DataType.Suits:
                savedHash = GameController.SuitsHash;
                return LoginManager.User.SuitsHash != savedHash;
            case DataType.Trades:
                savedHash = GameController.TradesHash;
                return LoginManager.User.TradesHash != savedHash;
            case DataType.UserInfo:
                return !LoginManager.LocalUser;
        }

        return true;
    }

    public static IEnumerator SendImage(string fileName, string URL)
    {
        WWW localFile = new WWW("file://" + fileName);
        yield return localFile;
        if (localFile.error == null)
            Debug.Log("Loaded file successfully" + localFile.texture == null);
        else
        {
            Debug.Log("Open file error: " + localFile.error);
            yield break; // stop the coroutine here
        }

        if (localFile.texture == null)
        {
            Debug.Log("Texture error");
            yield break;
        }

        WWWForm postForm = new WWWForm();
        postForm.AddBinaryData("theFile", localFile.bytes, fileName, "image/png");

        var headers = postForm.headers;
        headers["Authorization"] = LoginManager.Instance.Headers.Find(h => h.Name == "Authorization").Value;
        byte[] rawData = postForm.data;

        WWW upload = new WWW(URL, rawData, headers);
        yield return upload;
        if (upload.error == null)
            Debug.Log("upload done :");
        else
            Debug.Log("Error during upload: " + upload.error + " URL: " + URL);

        LoginManager.Instance.GetAvatarImage();
    }
}

public class ErrorAnswer
{
    public string Message { get; set; }
    public Dictionary<string, IList<string>> ModelState { get; set; }
}


public class AnswerModel
{
    public string Text { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public Dictionary<string, IList<string>> Errors { get; set; }

    public AnswerModel() { }

    public AnswerModel(string text)
    {
        Text = text;
        StatusCode = HttpStatusCode.OK;
        Errors = null;
    }

    public void SetFields(AnswerModel toCopy)
    {
        Text = toCopy.Text;
        StatusCode = toCopy.StatusCode;
        Errors = toCopy.Errors;
    }
}

public class InputString
{
    public string Value { get; set; }
}

public class AccessToken
{
    [JsonProperty(PropertyName = "access_token")]
    public string Token { get; set; }

    [JsonProperty(PropertyName = "userName")]
    public string Email { get; set; }

    [JsonProperty(PropertyName = "refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty(PropertyName = "refresh_expires_in")]
    public float RefreshExpireIn { get; set; }

    [JsonProperty(PropertyName = "expires_in")]
    public float TokenExpiresIn { get; set; }
}

public class Header
{
    public string Name { get; set; }
    public string Value { get; set; }

    public Header() { }

    public Header(string header, string val)
    {
        Name = header;
        Value = val;
    }
}

public class InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameRu { get; set; }
    public int Cost { get; set; }
    public string Type { get; set; }
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public bool Tradable { get; set; }
}

public class BonusUpgrade
{
    public string BonusName { get; set; }
    public int UpgradeAmount { get; set; }
    public float BonusTime
    {
        get
        {
            return 6 * Mathf.Pow(1.15f, UpgradeAmount);
        }
    }
}

public class WeeklyTaskModel
{
    public int Id { get; set; }
    public System.DateTime BeginDate { get; set; }
    public System.DateTime ExpireDate { get; set; }
    public int ActionCount { get; set; }
    public bool InOneRun { get; set; }
    public string Type { get; set; }
    public int TaskId { get; set; }
}

public class TournamentModel
{
    public int Id { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public string Name { get; set; }
    public string Prizes { get; set; }
    public bool AvaliableWeeklyTasks { get; set; }
    public List<FriendOfferStatisticsModel> TournamentLeaders { get; set; }
}

public class ItemBuyModel
{
    public int ItemId { get; set; }
    public int Amount { get; set; }
}

public class SubmitModel
{
    public string Key { get; set; }
}

public class SubmitScoreModel : SubmitModel
{
    public int IceCreamCount { get; set; }
    public int CasesCount { get; set; }
    public int Distance { get; set; }
    public bool NotContinued { get; set; }
    public Dictionary<int, byte> Uses { get; set; }
}

public class SubmitDuelScoreModel
{
    public int Distance { get; set; }
    public string DistanceString { get; set; }
    public int Id { get; set; }
}

public class SubmitAchievementModel
{
    public int Id { get; set; }
    public int AchievementId { get; set; }
    public int PlayerProgress { get; set; }
    public int Language { get; set; }
}

public class PlayerTasksAnswer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Bonus Reward { get; set; }
}

public class SubmitTaskModel
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int PlayerProgress { get; set; }
}

public class UserInfoModel
{
    public int Id { get; set; }
    public int IceCream { get; set; }
    public int HighScore { get; set; }
    public string Nickname { get; set; }
    public int IncomingFriendships { get; set; }
    public int IncomingTrades { get; set; }
    public int IncomingDuels { get; set; }
    public int NewFriendships { get; set; }
    public int NewTrades { get; set; }
    public int NewDuels { get; set; }
    public int DaysInRow { get; set; }
    public bool GotDailyBonus { get; set; }
    public int DuelWins { get; set; }
    public string Region { get; set; }
    public bool CanOfferTrade { get; set; }
    public DateTime? BunnedUntil { get; set; }
    public int Cases { get; set; }
    public int CaseId { get; set; }
    public List<PlayerTasks> Achievements { get; set; }
    public List<PlayerTasks> WeeklyTasks { get; set; }
    public List<BonusUpgrade> BonusUpgrades { get; set; }
    public List<Bonus> Bonuses { get; set; }
    public string ShopHash { get; set; }
    public string DuelsHash { get; set; }
    public string FriendsHash { get; set; }
    public string SuitsHash { get; set; }
    public string TradesHash { get; set; }

    public UserInfoModel()
    {
        Achievements = new List<PlayerTasks>();
        WeeklyTasks = new List<PlayerTasks>();
        BonusUpgrades = new List<BonusUpgrade>();
        Bonuses = new List<Bonus>();
    }
}

public class Bonus
{
    public string Type { get; set; }
    public int Amount { get; set; }
    public int Id { get; set; }
    public NameLocalization Name { get; set; }
}

public class NameLocalization
{
    public string Name { get; set; }
    public string NameRu { get; set; }

    public NameLocalization() { }

    public NameLocalization(string name, string nameRu)
    {
        Name = name;
        NameRu = nameRu;
    }

    public void SetNames(string name, string nameRu)
    {
        Name = name;
        NameRu = NameRu;
    }

}

public class InputInt
{
    public int Value { get; set; }
}

public class PlayerTasks
{
    public int Id { get; set; }
    public int PlayerProgress { get; set; }
    public int ActionsCount { get; set; }
    public string Type { get; set; }
    public bool InOneRun { get; set; }
    public int PlayerStartProgress { get; set; }
    /// <summary>
    /// achievements or weekly task id
    /// </summary>
    public int TaskId { get; set; }

    public string GenerateDescription()
    {
        StringBuilder description = new StringBuilder();

        switch (Type)
        {
            case "Run":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("runtask"), ActionsCount);
                break;
            case "Jump":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("jumptask"), ActionsCount);
                break;
            case "CollectIceCream":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("collecticecreamtask"), ActionsCount);
                break;
            case "Buy":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("buytask"), ActionsCount);
                break;
            case "Loose":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("loosetask"), ActionsCount);
                break;
            case "Play":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("playtask"), ActionsCount);
                break;
            case "CollectBonus":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("collectbonustask"), ActionsCount);
                break;
            case "ShareVK":
                description.Append(LocalizationManager.GetLocalizedValue("sharetaskvk"));
                break;
            case "ShareFB":
                description.Append(LocalizationManager.GetLocalizedValue("sharetaskfb"));
                break;
            case "ShareOK":
                description.Append(LocalizationManager.GetLocalizedValue("sharetaskok"));
                break;
        }

        if (InOneRun)
        {
            description.Append(LocalizationManager.GetLocalizedValue("inonerun"));
        }

        return description.ToString();
    }
}

public class PlayerAchievementModel : PlayerTasks
{
    public string Name { get; set; }
    public int Reward { get; set; }
    public DateTime? CompleteDate { get; set; }
}

public class PlayerTaskModel : PlayerTasks
{
    public int Reward { get; set; }
    public DateTime ExpireDate { get; set; }
}

public class UsersSearchModel
{
    public List<FriendRequestModel> Users { get; set; }
    public int TotalCount;

    public UsersSearchModel()
    {
        Users = new List<FriendRequestModel>();
    }
}

public class FriendsSearchModel
{
    public List<FriendModel> Users { get; set; }
    public int TotalCount;

    public FriendsSearchModel()
    {
        Users = new List<FriendModel>();
    }
}

public class FriendRequestModel
{
    public int Id { get; set; }
    public string Nickname { get; set; }
}

public class FriendModel
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public int Highscore { get; set; }
    public int IceCream { get; set; }
    public int DuelWins { get; set; }
    public string Region { get; set; }
}

public class FullFriendInfoModel
{
    public List<FriendModel> Friends { get; set; }
    public List<FriendModel> FriendRequests { get; set; }
    public string FriendsHash { get; set; }
}

public class PlayerSearchModel
{
    public string SearchString { get; set; }
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
}

public class FriendOfferModel
{
    public int Id { get; set; }
    public int Place { get; set; }
    public string Nickname { get; set; }
    public int Highscore { get; set; }
    public int DuelWins { get; set; }
    public int IceCream { get; set; }
    public string Region { get; set; }
    public int FriendshipStatus { get; set; }
}

public class FriendOfferStatisticsModel : FriendOfferModel
{
    public int Points { get; set; }
}

public class DuelModel
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public int Bet { get; set; }
    public int? Result { get; set; }
    public int UserId { get; set; }
    public DateTime ExpireDate { get; set; }
    public int Status { get; set; }
}

public class DuelsFullInfoModel
{
    public List<DuelModel> DuelOffers { get; set; }
    public List<DuelModel> DuelRequests { get; set; }
    public string DuelsHash { get; set; }
}

public class TradeItemsModel
{
    public List<InventoryCard> Cards { get; set; }
    public List<InventoryItem> Bonuses { get; set; }
    public InventoryItem IceCream { get; set; }

    public TradeItemsModel()
    {
        Cards = new List<InventoryCard>();
        Bonuses = new List<InventoryItem>();
        IceCream = new InventoryItem();
    }
}

public class TradeOfferModel
{
    public InventoryItem OfferItem { get; set; }
    public InventoryItem RequestItem { get; set; }
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Nickname { get; set; }
}

public class TradeModel
{
    public List<TradeOfferModel> Trades { get; set; }
    public string TradesHash { get; set; }
}

public class DuelOfferModel
{
    public int Id { get; set; }
    public int Bet { get; set; }
}

public class ShopItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Cost { get; set; }
    public int Amount { get; set; }
    public string NameRu { get; set; }
}

public class Costume
{
    public int CostumeId { get; set; }
    public string Name { get; set; }
    public string NameRu { get; set; }
    public int CostumeAmount { get; set; }
    public List<InventoryCard> Cards { get; set; }

    public Costume()
    {
        Cards = new List<InventoryCard>();
    }
}

public class SuitsModel
{
    public List<Costume> Costumes { get; set; }
    public string SuitsHash { get; set; }

    public SuitsModel()
    {
        Costumes = new List<Costume>();
    }
}

public class InventoryCard : InventoryItem
{
    public int SuitId { get; set; }
    public byte Position { get; set; }
}

public class ShopCard : ShopItem
{
    public int SuitId { get; set; }
    public int Position { get; set; }
    public string SuitName { get; set; }
}

public class ShopModel
{
    public List<ShopCard> Cards { get; set; }
    public List<ShopItem> Bonuses { get; set; }
    public List<ShopItem> Cases { get; set; }
    public List<ShopItem> BonusUpgrades { get; set; }
    public string ShopHash { get; set; }
}

public class DuelResultModel
{
    public PlayerDuelModel FirstPlayer { get; set; }
    public PlayerDuelModel SecondPlayer { get; set; }
    public int Prize { get; set; }
}

public class PlayerDuelModel
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public int Result { get; set; }
}


public class CheckTokenModel
{
    public string Email { get; set; }

    public string Code { get; set; }
}

public class ResetPasswordViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Code { get; set; }
}

public class UserInfoViewModel
{
    public string Email { get; set; }
    public bool HasRegistered { get; set; }
    public string LoginProvider { get; set; }
}

public class AdsModel
{
    public int Id { get; set; }
    public string Text { get; set; }
}

public class RegisterExternalBindingModel
{
    public RegisterExternalBindingModel(RegisterBindingModel model)
    {
        Email = model.Email;
        Nickname = model.Nickname;
        PhoneNumber = model.PhoneNumber;
        RegionId = model.RegionId;
    }
    public string Email { get; set; }
    public string Nickname { get; set; }
    public string PhoneNumber { get; set; }
    public int RegionId { get; set; }
    public int IceCream { get; set; }
    public int Cases { get; set; }
    public List<Bonus> Bonuses { get; set; }
    public List<BonusUpgrade> BonusUpgrades { get; set; }
}

public class RegionModel
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string PhonePattern { get; set; }
    public string PhonePlaceholder { get; set; }
}
