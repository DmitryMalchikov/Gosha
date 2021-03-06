﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Facebook.Unity;
using Odnoklassniki;
using System.Text.RegularExpressions;
using System.Linq;

public delegate void ResultCallback();

public class LoginManager : Singleton<LoginManager>
{
    public static AccessToken userToken;
    public static string LoginProvider;

    public Button LoginBtn;
    public Button SendResetCodeBtn;

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
    public string ShareImageUrl = "http://gosha.by/Resources/ShareScreen.png";

    public string GetRegionsUrl = "/api/account/regions";

    public InputField Email;
    public InputField Password;

    public List<Header> Headers = new List<Header>();

    public static UserInfoModel User;
    public static bool LocalUser = false;

    public void SetUrls()
    {
        ServerInfo.SetUrl(ref LoginUrl);
        ServerInfo.SetUrl(ref RegisterUrl);
        ServerInfo.SetUrl(ref UserInfoUrl);
        ServerInfo.SetUrl(ref ProfileImageUrl);
        ServerInfo.SetUrl(ref ForgotPasswordUrl);
        ServerInfo.SetUrl(ref ValidateResetTokenUrl);
        ServerInfo.SetUrl(ref ResetPasswordUrl);
        ServerInfo.SetUrl(ref RegisterUserInfoUrl);
        ServerInfo.SetUrl(ref ExternalRegisterUrl);
        ServerInfo.SetUrl(ref ImageUploadUrl);
        ServerInfo.SetUrl(ref GetRegionsUrl);
    }

    private IEnumerator Start()
    {
        SetUrls();

        string tokenExpires = PlayerPrefs.GetString("token_expires_in_gosha");

        yield return new WaitUntil(() => AdsManager.Initialized);

        if (!string.IsNullOrEmpty(tokenExpires))
        {
            DateTime tokenExpireDate = DateTime.Parse(tokenExpires);
            if (tokenExpireDate > DateTime.Now.AddDays(1))
            {
                string token = PlayerPrefs.GetString("token_gosha");
                
                Headers = new List<Header>() { new Header("Authorization", " Bearer " + token) };
                GetUserInfoAsync();

                AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.CloseLoading();
                AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);

                LoginCanvas.Instance.EnableWarning(false);
                LoginCanvas.Instance.Enable(false);

                yield break;
            }
        }

        string refreshExpires = PlayerPrefs.GetString("refresh_expires_in_gosha");

        if (string.IsNullOrEmpty(refreshExpires))
        {
            LocalLogin();
        }
        else
        {
            DateTime refreshExpireDate = DateTime.Parse(refreshExpires);

            if (refreshExpireDate > DateTime.Now)
            {
                var refreshToken = PlayerPrefs.GetString("refresh_token_gosha");
                GetTokenByRefreshAsync(refreshToken);
            }
            else
            {
                var provider = PlayerPrefs.GetString("provider_gosha");

                if (!string.IsNullOrEmpty(provider))
                {
                    OpenExternalLogin(provider);
                }
                else
                {
                    LocalLogin();
                }
            }
        }
    }

    private void LocalLogin()
    {
        LocalUser = true;
        AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.CloseLoading();
        AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);
        LoginCanvas.Instance.EnableWarning(false);
        LoginCanvas.Instance.Enable(false);
        GetUserInfoAsync();
    }

    public void GetTokenByRefreshAsync(string refreshToken)
    {
        CoroutineManager.SendRequest(LoginUrl, string.Format("refresh_token={0}&grant_type=refresh_token", refreshToken), (AccessToken token) =>
           {
               LocalUser = false;
               userToken = token;

               OneSignal.SetSubscription(true);
               OneSignal.SyncHashedEmail(userToken.Email);

               PlayerPrefs.SetString("refresh_token_gosha", userToken.RefreshToken);
               PlayerPrefs.SetString("refresh_expires_in_gosha", DateTime.Now.AddSeconds(userToken.RefreshExpireIn).ToString());

               PlayerPrefs.SetString("token_gosha", userToken.Token);
               PlayerPrefs.SetString("token_expires_in_gosha", DateTime.Now.AddSeconds(userToken.TokenExpiresIn).ToString());

               LoginCanvas.Instance.EnableWarning(false);

               Debug.Log(userToken.Token);

               LoginCanvas.Instance.Enable(false);

               Headers = new List<Header>() { new Header("Authorization", " Bearer " + userToken.Token) };
               GetUserInfoAsync();
               //AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.ADSPanel.OpenAds();
               AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.CloseLoading();
               AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);               
           },
            (response) =>
            {
                //LoginCanvas.Instance.EnableWarning(true);
                Canvaser.Instance.CloseLoading();
            });
    }

    public void GetTokenAsync()
    {
        Canvaser.Instance.SBonuses.ResetStartBonuses();
        GetTokenAsync(Email.text, Password.text);
    }

    public void GetTokenAsync(string email, string password)
    {
        LoginBtn.interactable = false;

        CoroutineManager.SendRequest(LoginUrl, string.Format("username={0}&password={1}&grant_type=password", email, password), (AccessToken token) =>
       {
           Extensions.RemoveJsonData(DataType.UserInfo);
           LocalUser = false;
           userToken = token;

           PlayerPrefs.SetString("refresh_token_gosha", userToken.RefreshToken);
           PlayerPrefs.SetString("refresh_expires_in_gosha", DateTime.Now.AddSeconds(userToken.RefreshExpireIn).ToString());

           PlayerPrefs.SetString("token_gosha", userToken.Token);
           PlayerPrefs.SetString("token_expires_in_gosha", DateTime.Now.AddSeconds(userToken.TokenExpiresIn).ToString());

           OneSignal.SetSubscription(true);
           OneSignal.SyncHashedEmail(userToken.Email);

           LoginCanvas.Instance.EnableWarning(false);

           Debug.Log(userToken.Token);

           LoginCanvas.Instance.Enable(false);

           Headers = new List<Header>() { new Header("Authorization", " Bearer " + userToken.Token) };
           GetUserInfoAsync();
           //AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.ADSPanel.OpenAds();
           AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.CloseLoading();
           AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);           
       },
        (response) =>
        {
            LoginCanvas.Instance.EnableWarning(true);
        },
        finallyMethod:
        () =>
        {
            LoginBtn.interactable = true;
        });
    }

    public void GetUserInfo()
    {
        GetUserInfoAsync();
    }

    public void GetUserInfoAsync(ResultCallback callback = null)
    {
        CoroutineManager.SendRequest(UserInfoUrl, null, (UserInfoModel info) =>
        {
            if (info == null)
            {
                info = new UserInfoModel();
            }

            User = info;
            GameController.Instance.LoadBonusesTime(info.BonusUpgrades);
            Canvaser.Instance.SetAllIceCreams(User.IceCream);
            Canvaser.Instance.SBonuses.SetStartBonuses(info.Bonuses);
            Canvaser.Instance.SetNotifications(info);

            if (!LocalUser)
            {
                Canvaser.Instance.Nickname.text = User.Nickname;
                AchievementsManager.Instance.LoadAchievements(info.Achievements);
                TasksManager.Instance.LoadTasks(info.WeeklyTasks);                
                Canvaser.Instance.DailyBonus.SetHighlights();
                Canvaser.Instance.SettingsRegion.Key = User.Region;
                Canvaser.Instance.Shop.SetPromoBtn();

                if (!User.GotDailyBonus)
                {
                    Canvaser.Instance.DailyBonus.gameObject.SetActive(true);
                }
                GetAvatarImage();

                //NotificationsManager.Register(User.Id);

                Canvaser.Instance.OpenNotificationPanel(NotificationType.DuelRequest, User.NewDuels);
                Canvaser.Instance.OpenNotificationPanel(NotificationType.FriendRequest, User.NewFriendships);
                Canvaser.Instance.OpenNotificationPanel(NotificationType.TradeRequest, User.NewTrades);
            }

            if (callback != null)
            {
                callback();
            }
        }, type: DataType.UserInfo);
    }

    public void RegisterAsync(RegisterBindingModel model)
    {
        if (User != null)
        {
            model.IceCream = User.IceCream;
            model.Cases = User.Cases;
            model.Bonuses = User.Bonuses;
            model.BonusUpgrades = User.BonusUpgrades;
            Extensions.RemoveJsonData(DataType.UserInfo);
        }

        CoroutineManager.SendRequest(RegisterUrl, model, () =>
       {
           GetTokenAsync(model.Email, model.Password);
           Canvaser.Instance.RegistrationFinishedPanel.gameObject.SetActive(true);
           Canvaser.Instance.RegistrationPanel.PageNum = 1;
           Canvaser.Instance.RegistrationPanel.gameObject.SetActive(false);
       });
    }

    public void ForgotPasswordAsync(string email)
    {
        InputString data = new InputString() { Value = email };
        SendResetCodeBtn.interactable = false;
        CoroutineManager.SendRequest(ForgotPasswordUrl, data, () =>
       {
           Canvaser.Instance.ForgotPassword.OpenPasswordInputs();
       },
       finallyMethod: () =>
       {
           SendResetCodeBtn.interactable = true;
       });
    }

    public void ValidateResetTokenAsync(string email, string code)
    {
        CheckTokenModel data = new CheckTokenModel() { Email = email, Code = code };

        CoroutineManager.SendRequest(ValidateResetTokenUrl, data, (bool validated) =>
       {
           if (validated)
           {
               Canvaser.Instance.ForgotPassword.ResetPassword();
           }
           else
           {
               Canvaser.Instance.ForgotPassword.SetWarning(LocalizationManager.GetLocalizedValue("wrongcode"));
           }
       });
    }

    public void ResetPasswordAsync(string email, string password, string confirmPassword, string code)
    {
        ResetPasswordViewModel data = new ResetPasswordViewModel() { Email = email, Code = code, Password = password, ConfirmPassword = confirmPassword };

        CoroutineManager.SendRequest(ResetPasswordUrl, data,
        () =>
        {
            Canvaser.Instance.ForgotPassword.ResetFinished();
        },
        (response) =>
        {
            Canvaser.Instance.ForgotPassword.SetWarning(LocalizationManager.GetLocalizedValue(response.Errors.First().Value[0]));
        });
    }

    public void OpenExternalLogin(string provider)
    {
        Canvaser.Instance.SBonuses.ResetStartBonuses();
        LoginProvider = provider;
        SampleWebView.Instance.OpenWindow(string.Format(ExternalLoginUrl, provider));
    }

    public void CheckExternalRegister(string refresh, string expires, string email)
    {
        Headers = new List<Header>() { new Header("Authorization", " Bearer " + userToken.Token) };

        if (string.IsNullOrEmpty(refresh))
        {
            Canvaser.Instance.RegistrationPanel.ExternalRegistration(email);
        }
        else
        {
            LocalUser = false;
            var seconds = int.Parse(Regex.Replace(expires, "\\D", string.Empty));
            PlayerPrefs.SetString("provider_gosha", LoginProvider);
            PlayerPrefs.SetString("refresh_token_gosha", refresh);
            PlayerPrefs.SetString("refresh_expires_in_gosha", DateTime.Now.AddSeconds(seconds).ToString());
            OneSignal.SetSubscription(true);
            OneSignal.SyncHashedEmail(email);
            Canvaser.Instance.LoginPanel.SetActive(false);
            GetUserInfoAsync();
            Canvaser.Instance.MainMenu.SetActive(true);

            AdsManager.Instance.OnAdsDownloaded += () => Canvaser.Instance.CloseLoading();
            AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);
        }
    }

    public void RegisterExternal(RegisterExternalBindingModel model)
    {
        if (User != null)
        {
            model.IceCream = User.IceCream;
            model.Cases = User.Cases;
            model.Bonuses = User.Bonuses;
            model.BonusUpgrades = User.BonusUpgrades;
            Extensions.RemoveJsonData(DataType.UserInfo);
        }

        CoroutineManager.SendRequest(ExternalRegisterUrl, model, () =>
       {
           OpenExternalLogin(LoginProvider);
           Canvaser.Instance.RegistrationPanel.PageNum = 1;
           Canvaser.Instance.RegistrationFinishedPanel.gameObject.SetActive(true);
           Canvaser.Instance.RegistrationPanel.gameObject.SetActive(false);
           Canvaser.Instance.LoginPanel.gameObject.SetActive(false);
           Canvaser.Instance.MainMenu.SetActive(true);
       });
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("refresh_token_gosha");
        PlayerPrefs.DeleteKey("refresh_expires_in_gosha");
        PlayerPrefs.DeleteKey("provider_gosha");
        PlayerPrefs.DeleteKey("CurrentSuit");
        PlayerPrefs.DeleteKey("token_gosha");
        PlayerPrefs.DeleteKey("token_expires_in_gosha");
        PlayerController.Instance.TakeOffSuits();
        OneSignal.SetSubscription(false);
        FB.LogOut();
        if (OK.IsInitialized)
        {
            OK.Logout();
        }
        VK.LogOut();
        Headers = new List<Header>();
        userToken = null;
        User = new UserInfoModel();
        LocalUser = true;
        Canvaser.Instance.SetAllIceCreams(User.IceCream);
        Canvaser.Instance.SetNotifications(User);
        GameController.Instance.ResetBonusesTime();
        Canvaser.Instance.SBonuses.ResetStartBonuses();
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

        var headers = Headers.ToDictionary(h => h.Name, h => h.Value);// new Dictionary<string, string>() { { Headers[0].Name, Headers[0].Value } };

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
                img.sprite = Resources.Load<Sprite>("iTunesArtwork");
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
            Canvaser.Instance.SetAvatar(Resources.Load<Sprite>("iTunesArtwork"));
        }
    }

    public void GetRegions()
    {
        CoroutineManager.SendRequest(GetRegionsUrl, null, (List<RegionModel> regions) =>
       {
           Canvaser.Instance.RegistrationPanel.SetRegions(regions);
       });
    }
}
