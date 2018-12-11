using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.DTO;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Network;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using Facebook.Unity;
using Odnoklassniki;
using UnityEngine;
using UnityEngine.UI;
using AccessToken = Assets.Scripts.DTO.AccessToken;

namespace Assets.Scripts.Managers
{
    public delegate void ResultCallback();

    public class LoginManager : Singleton<LoginManager>, IAvatarSprite
    {
        public static AccessToken UserToken;
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
                    
                    AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);

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
            AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);
            LoginCanvas.Instance.Enable(false);
            GetUserInfoAsync();
        }

        public void GetTokenByRefreshAsync(string refreshToken)
        {
            CoroutineManager.SendRequest(LoginUrl, string.Format("refresh_token={0}&grant_type=refresh_token", refreshToken), 
                (Action<AccessToken>)HandleUserRefreshToken,
                (response) =>
                {
                    //LoginCanvas.Instance.EnableWarning(true);
                    LocalLogin();
                    //Canvaser.Instance.CloseLoading();
                });
        }

        public void HandleUserRefreshToken(AccessToken token)
        {
            LocalUser = false;
            UserToken = token;

            OneSignal.SetSubscription(true);
            OneSignal.SyncHashedEmail(UserToken.Email);

            PlayerPrefs.SetString("refresh_token_gosha", UserToken.RefreshToken);
            PlayerPrefs.SetString("refresh_expires_in_gosha", DateTime.Now.AddSeconds(UserToken.RefreshExpireIn).ToString());

            PlayerPrefs.SetString("token_gosha", UserToken.Token);
            PlayerPrefs.SetString("token_expires_in_gosha", DateTime.Now.AddSeconds(UserToken.TokenExpiresIn).ToString());

            LoginCanvas.Instance.Enable(false);

            Headers = new List<Header>() { new Header("Authorization", " Bearer " + UserToken.Token) };
            GetUserInfoAsync();
            AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);
        }

        public void GetTokenAsync()
        {
            Canvaser.Instance.SBonuses.ResetStartBonuses();
            GetTokenAsync(Email.text, Password.text);
        }

        public void GetTokenAsync(string email, string password)
        {
            LoginBtn.interactable = false;

            CoroutineManager.SendRequest(LoginUrl, string.Format("username={0}&password={1}&grant_type=password", email, password),
                (Action<AccessToken>)HandleUserToken,
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

        public void HandleUserToken(AccessToken token)
        {
            FileExtensions.RemoveJsonData(DataType.UserInfo);
            HandleUserRefreshToken(token);
        }

        public void GetUserInfo()
        {
            GetUserInfoAsync();
        }

        public CustomTask GetUserInfoAsync(ResultCallback callback = null)
        {
            CustomTask task = new CustomTask();
            CoroutineManager.SendRequest(UserInfoUrl, null, (UserInfoModel info) =>
                {
                    HandleUserInfo(info);

                    if (callback != null)
                    {
                        callback();
                    }
                },
                type: DataType.UserInfo,
                finallyMethod: () =>
                {
                    if (User == null)
                    {
                        SetUserInfos(User);
                    }

                    FileExtensions.SaveJsonDataAsync(DataType.UserInfo, User);
                    task.Ready = true;
                });

            return task;
        }

        public void HandleUserInfo(UserInfoModel info)
        {
            SetUserInfos(info);

            if (LocalUser) return;

            Canvaser.Instance.Nickname.text = User.Nickname;
            AchievementsManager.Instance.LoadAchievements(info.Achievements);
            TasksManager.Instance.LoadTasks(info.WeeklyTasks);
            Canvaser.Instance.DailyBonus.Show(!User.GotDailyBonus);
            Canvaser.Instance.SettingsRegion.Key = User.Region;
            Canvaser.Instance.Shop.SetPromoBtn();

            GetUserImage();
            //GetAvatarImage();

            //NotificationsManager.Register(User.Id);

            Canvaser.Instance.OpenNotificationPanel(NotificationType.DuelRequest, User.NewDuels);
            Canvaser.Instance.OpenNotificationPanel(NotificationType.FriendRequest, User.NewFriendships);
            Canvaser.Instance.OpenNotificationPanel(NotificationType.TradeRequest, User.NewTrades);
        }

        public void SetUserInfos(UserInfoModel user)
        {
            if (user == null)
            {
                user = new UserInfoModel();
            }

            User = user;
            GameController.Instance.LoadBonusesTime(user.BonusUpgrades);
            Canvaser.Instance.SetAllIceCreams(user.IceCream);
            Canvaser.Instance.SBonuses.SetStartBonuses(user.Bonuses);
            Canvaser.Instance.SetNotifications(user);
            LocalUser = string.IsNullOrEmpty(user.Nickname);
        }

        public void RegisterAsync(RegisterBindingModel model)
        {
            PopulateLocalUser(model);

            CoroutineManager.SendRequest(RegisterUrl, model, () =>
            {
                GetTokenAsync(model.Email, model.Password);
                Canvaser.RegistrationFinished();
            });
        }

        public void PopulateLocalUser(RegisterExternalBindingModel model)
        {
            if (User == null) return;

            model.IceCream = User.IceCream;
            model.Cases = User.Cases;
            model.Bonuses = User.Bonuses.ToArray();
            model.BonusUpgrades = User.BonusUpgrades.ToArray();
            FileExtensions.RemoveJsonData(DataType.UserInfo);
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
            Headers = new List<Header>() { new Header("Authorization", " Bearer " + UserToken.Token) };

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
                
                AdsManager.Instance.GetAds(Canvaser.Instance.ADSPanel.txt, Canvaser.Instance.ADSPanel.img);
            }
        }

        public void RegisterExternal(RegisterExternalBindingModel model)
        {
            PopulateLocalUser(model);

            CoroutineManager.SendRequest(ExternalRegisterUrl, model, () =>
            {
                OpenExternalLogin(LoginProvider);
                Canvaser.RegistrationFinished();
                Canvaser.Instance.LoginPanel.gameObject.SetActive(false);
                Canvaser.Instance.MainMenu.SetActive(true);
            });
        }

        public void LogOut()
        {
            ClearSavedData();
            SuitsManager.TakeOffSuits();
            OneSignal.SetSubscription(false);
            FB.LogOut();
            if (OK.IsInitialized)
            {
                OK.Logout();
            }
            VK.LogOut();
            Headers = new List<Header>();
            UserToken = null;
            User = new UserInfoModel();
            LocalUser = true;
            Canvaser.Instance.SetAllIceCreams(User.IceCream);
            Canvaser.Instance.SetNotifications(User);
            GameController.Instance.ResetBonusesTime();
            Canvaser.Instance.SBonuses.ResetStartBonuses();
        }

        private void ClearSavedData()
        {
            PlayerPrefs.DeleteKey("refresh_token_gosha");
            PlayerPrefs.DeleteKey("refresh_expires_in_gosha");
            PlayerPrefs.DeleteKey("provider_gosha");
            PlayerPrefs.DeleteKey("CurrentSuit");
            PlayerPrefs.DeleteKey("token_gosha");
            PlayerPrefs.DeleteKey("token_expires_in_gosha");
            FileExtensions.RemoveAllCachedData();
        }

        public void SendImage(string path)
        {
            StartCoroutine(NetworkHelper.SendImage(path, ImageUploadUrl));
        }

        public void GetUserImage(IAvatarSprite spriteSetter = null, int userId = 0)
        {
            if (userId == 0)
            {
                userId = User.Id;
            }

            if (spriteSetter == null)
            {
                spriteSetter = this;
            }

            StartCoroutine(DownloadImage(userId, spriteSetter));
        }

        IEnumerator DownloadImage(int userId, IAvatarSprite spriteSetter)
        {
            string url = ProfileImageUrl + userId;

            WWW www = new WWW(url);

            yield return www;

            if (spriteSetter == null) yield break;

            if (www.error == null)
            {
                spriteSetter.SetSprite(Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.one * 0.5f));
            }
            else
            {
                spriteSetter.SetSprite(Resources.Load<Sprite>("iTunesArtwork"));
            }
        }

        public void GetRegions()
        {
            CoroutineManager.SendRequest(GetRegionsUrl, null, (List<RegionModel> regions) =>
            {
                Canvaser.Instance.RegistrationPanel.SetRegions(regions);
            });
        }

        public void SetSprite(Sprite sprite)
        {
            Canvaser.Instance.SetAvatar(sprite);
        }
    }
}