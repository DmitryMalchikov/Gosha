using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ResultCallback();

public class LoginManager : MonoBehaviour
{

    public static LoginManager Instance { get; private set; }
    public static AccessToken userToken;
    public static string LoginProvider;

    public RawImage Image;
    public Text Error;

    public string LoginUrl = "/token";
    public string RegisterUrl = "/api/account/register";
    public string UserInfoUrl = "/api/user/userinfo";
    public string ProfileImageUrl = "/api/user/profileimage?userId=";
    public string ForgotPasswordUrl = "/api/account/forgotpassword";
    public string ValidateResetTokenUrl = "/api/account/validatepasswordresettoken";
    public string ResetPasswordUrl = "/api/account/ResetPassword";
    public string RegisterUserInfoUrl = "/api/account/UserInfo";
    public string ExternalRegisterUrl = "/api/account/RegisterExternal";
    public string ImageUploadUrl = "/api/user/UploadImage";
    public string ExternalLoginUrl = "/api/Account/ExternalLogin?provider={0}&response_type=token&client_id=self&redirect_uri=http%3A%2F%2F10.10.0.91.xip.io%3A58629%2FHtml%2FLogins.html&state=nBr4ZiNZQ6-0cA6x-7rBx6e8dM7na5HMzvm6EyroYNA1";

    public string GetRegionsUrl = "/api/account/regions";

    public InputField Email;
    public InputField Password;

    public List<Header> Headers;

    string _resetEmail;
    string _resetCode;

    public UserInfoModel User;

    public void SetUrls()
    {
        LoginUrl = ServerInfo.GetUrl(LoginUrl);
        RegisterUrl = ServerInfo.GetUrl(RegisterUrl);
        UserInfoUrl = ServerInfo.GetUrl(UserInfoUrl);
        ProfileImageUrl = ServerInfo.GetUrl(ProfileImageUrl);
        ForgotPasswordUrl = ServerInfo.GetUrl(ForgotPasswordUrl);
        ValidateResetTokenUrl = ServerInfo.GetUrl(ValidateResetTokenUrl);
        ResetPasswordUrl = ServerInfo.GetUrl(ResetPasswordUrl);
        RegisterUserInfoUrl = ServerInfo.GetUrl(RegisterUserInfoUrl);
        ExternalRegisterUrl = ServerInfo.GetUrl(ExternalRegisterUrl);
        ImageUploadUrl = ServerInfo.GetUrl(ImageUploadUrl);
        ExternalLoginUrl = ServerInfo.GetUrl(ExternalLoginUrl);
        GetRegionsUrl = ServerInfo.GetUrl(GetRegionsUrl);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetUrls();
        LoginProvider = PlayerPrefs.GetString("provider");
        if (!string.IsNullOrEmpty(LoginProvider))
        {
            OpenExternalLogin(LoginProvider);
        }
        else
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("email")))
            {
                Canvaser.Instance.CloseLoading();
                //Canvaser.Instance.LoginPanel.SetActive(true);
                //Canvaser.Instance.LoadingPanel.transform.SetAsLastSibling();
            }
            else
            {
                GetTokenAsync(true);
            }
        }
    }

    

    public void GetTokenAsync(bool haveCredits)
    {
        string email, password;

        if (haveCredits)
        {
            email = PlayerPrefs.GetString("email");
            password = PlayerPrefs.GetString("password");
        }
        else
        {
            email = Email.text;
            password = Password.text;
        }

        StartCoroutine(NetworkHelper.SendRequest(LoginUrl, string.Format("username={0}&password={1}&grant_type=password", email, password), "application/json", (response) =>
        {
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("password", password);
            OneSignal.SyncHashedEmail(email);

            LoginCanvas.Instance.EnableWarning(false);

            userToken = JsonConvert.DeserializeObject<AccessToken>(response.Text);

            Debug.Log(userToken.Token);

            LoginCanvas.Instance.Enable(false);

            Headers = new List<Header>() { new Header("Authorization", " Bearer " + userToken.Token) };
            GetUserInfoAsync();
            AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.ADSPanel.OpenAds();
            AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.CloseLoading();
            AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);
        },
        (response) =>
        {
            LoginCanvas.Instance.EnableWarning(true);
        }));
    }

    public void GetUserInfo()
    {
        GetUserInfoAsync();
    }

    public void GetUserInfoAsync(ResultCallback callback=null)
    {
        StartCoroutine(NetworkHelper.SendRequest(UserInfoUrl, "", "application/x-www-form-urlencoded", (response) =>
        {
            UserInfoModel info = JsonConvert.DeserializeObject<UserInfoModel>(response.Text);
            User = info;
            Canvaser.Instance.SBonuses.SetStartBonuses(info.Bonuses);
            AchievementsManager.Instance.LoadAchievements(info.Achievements);
            TasksManager.Instance.LoadTasks(info.WeeklyTasks);
            Canvaser.Instance.SetNotifications(info);
            //Canvaser.Instance.Shop.UpdatePanel(info.IceCream);
            Canvaser.Instance.SetAllIceCreams(User.IceCream);
            GameController.Instance.LoadBonusesTime(info.BonusUpgrades);
            Canvaser.Instance.DailyBonus.SetHighlights();
            Canvaser.Instance.SettingsRegion.Key = User.Region;
            Canvaser.Instance.Shop.SetPromoBtn();
            if (!User.GotDailyBonus)
            {
                Canvaser.Instance.DailyBonus.gameObject.SetActive(true);
            }
            GetAvatarImage();

            if (callback != null)
            {
                callback();
            }
        }));
    }

    //NOT DONE!!!
    public void RegisterAsync(RegisterBindingModel model)
    {
        StartCoroutine(NetworkHelper.SendRequest(RegisterUrl, JsonConvert.SerializeObject(model), "application/json", (response) =>
        {
            Canvaser.Instance.RegistrationFinishedPanel.gameObject.SetActive(true);
            Canvaser.Instance.RegistrationPanel.gameObject.SetActive(false);
        }));
    }

    public void ForgotPasswordAsync(string email)
    {
        InputString data = new InputString() { Value = email };
        StartCoroutine(NetworkHelper.SendRequest(ForgotPasswordUrl, JsonConvert.SerializeObject(data), "application/json", (response) =>
        {
            Debug.Log(response.Text);
            Canvaser.Instance.ForgotPassword.OpenPasswordInputs();
        }));
    }

    public void ValidateResetTokenAsync(string email, string code)
    {
        CheckTokenModel data = new CheckTokenModel() { Email = email, Code = code };

        StartCoroutine(NetworkHelper.SendRequest(ValidateResetTokenUrl, JsonConvert.SerializeObject(data), "application/json", (response) =>
        {
            Debug.Log(response.Text);

            bool validated = JsonConvert.DeserializeObject<bool>(response.Text);

            if (validated)
            {
                Canvaser.Instance.ForgotPassword.ResetPassword();
            }
            else
            {
                Canvaser.Instance.ForgotPassword.SetWarning(LocalizationManager.GetLocalizedValue("wrongcode"));
            }
        }));
    }

    public void ResetPasswordAsync(string email, string password, string confirmPassword, string code)
    {
        ResetPasswordViewModel data = new ResetPasswordViewModel() { Email = email, Code = code, Password = password, ConfirmPassword = confirmPassword };

        StartCoroutine(NetworkHelper.SendRequest(ResetPasswordUrl, JsonConvert.SerializeObject(data), "application/json", (response) =>
        {
            Debug.Log(response.Text);
            Canvaser.Instance.ForgotPassword.ResetFinished();
        },
        (response) =>
        {
            Canvaser.Instance.ForgotPassword.SetWarning(response.Text);
        }));
    }

    public void OpenExternalLogin(string provider)
    {
        LoginProvider = provider;
        SampleWebView.Instance.OpenWindow(string.Format(ExternalLoginUrl, provider));
    }

    public void CheckExternalRegister()
    {
        Headers = new List<Header>() { new Header() { Name = "Authorization", Value = "Bearer " + userToken.Token } };

        StartCoroutine(NetworkHelper.SendRequest(RegisterUserInfoUrl, "", "application/json", (response) =>
        {
            var result = JsonConvert.DeserializeObject<UserInfoViewModel>(response.Text);

            //PlayerPrefs.SetString("email", result.Email);

            if (!result.HasRegistered)
            {
                Canvaser.Instance.RegistrationPanel.ExternalRegistration(result.Email);
            }
            else
            {
                PlayerPrefs.SetString("provider", LoginProvider);
                Canvaser.Instance.LoginPanel.SetActive(false);
                GetUserInfoAsync();
                Canvaser.Instance.MainMenu.SetActive(true);
            }
        }));
    }

    public void RegisterExternal(RegisterExternalBindingModel model)
    {
        StartCoroutine(NetworkHelper.SendRequest(ExternalRegisterUrl, JsonConvert.SerializeObject(model), "application/json", (response) =>
        {

            //PlayerPrefs.SetString("provider", LoginProvider);
            GetUserInfoAsync();
            OpenExternalLogin(LoginProvider);
            Canvaser.Instance.RegistrationFinishedPanel.gameObject.SetActive(true);
            Canvaser.Instance.RegistrationPanel.gameObject.SetActive(false);
            Canvaser.Instance.LoginPanel.gameObject.SetActive(false);
            Canvaser.Instance.MainMenu.SetActive(true);
        }));
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("email");
        PlayerPrefs.DeleteKey("password");
        PlayerPrefs.DeleteKey("provider");
        OneSignal.SetSubscription(false);
        Headers = null;
        userToken = null;
    }

    public void SendImage(string path)
    {
        Debug.Log("Send Image: " + path);
		StartCoroutine(NetworkHelper.SendImage(path, ImageUploadUrl));
    }

    public void GetUserImage(int userId, Image img)
    {
        StartCoroutine(DownloadImage(userId, img));
    }

    IEnumerator DownloadImage(int userId, Image img)
    {
        string url = ProfileImageUrl + userId;

        var headers = new Dictionary<string, string>() { { Headers[0].Name, Headers[0].Value } };

        //WWW upload = new WWW(url, null, headers);//new WWW(URL, postForm);

        WWW www = new WWW(url);

        yield return www;

        if (img != null)
        {
            if (www.error == null)
            {
                img.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f);
            }
            else
            {
                img.sprite = Resources.Load<Sprite>("Avatar");
            }
        }
    }

    public void GetAvatarImage()
    {
        StartCoroutine(DownloadImage());
    }

    IEnumerator DownloadImage()
    {
        string url = ProfileImageUrl + User.Id;

        var headers = new Dictionary<string, string>() { { Headers[0].Name, Headers[0].Value } };

        WWW www = new WWW(url, null, headers);

        yield return www;
        if (www.error == null)
        {
            Canvaser.Instance.SetAvatar(Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f));
        }
        else
        {
            Canvaser.Instance.SetAvatar(Resources.Load<Sprite>("Avatar"));
        }

    }

    public void GetRegions()
    {

        StartCoroutine(NetworkHelper.SendRequest(GetRegionsUrl, "", "application/json", (response) =>
        {
            Canvaser.Instance.RegistrationPanel.SetRegions(JsonConvert.DeserializeObject<List<RegionModel>>(response.Text));
        }));

    }
}
