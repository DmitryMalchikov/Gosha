using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class Registration : MonoBehaviour
    {
        public string CheckEmailUrl = "/api/account/checkemail";
        public string CheckNickUrl = "/api/account/checknickname";
        public string CheckPhoneUrl = "/api/account/checkphone";

        public Animator pages;
        public Animator content;

        public byte PageNum = 0;

        public InputField Email;
        public InputField Phone;
        public InputField Nick;
        public InputField Password;
        public InputField ConfirmPassword;
        public Warning WarningPanel;
        public string InvalidFormatEmail;
        public string InvalidFormatPhone;
        public string InvalidPassword;
        public string EmailAlreadyExists;
        public string PhoneAlreadyExists;
        public string NickAlreadyExists;
        public string PasswordsMismatch;

        public RegisterBindingModel NewUser = new RegisterBindingModel();
        public List<RegionModel> regions = new List<RegionModel>();

        public RegionModel Region;
        public Transform RegionsContent;
        public GameObject RegionObject;
        public ToggleGroup Group;
        public GameObject LangPanel;
        public string NickCantBeEmpty;

        private string _phone;
        private PhoneNumberTemplate _phoneNumberHandler;
        private List<Action> _checks; 

        public void Start()
        {
            SetUrls();
            _phoneNumberHandler = new PhoneNumberTemplate();
            _checks = new List<Action> { CheckRegion, CheckEmail, CheckNick, ComparePasswords };
        }

        public void SetUrls()
        {
            ServerInfo.SetUrl(ref CheckEmailUrl);
            ServerInfo.SetUrl(ref CheckNickUrl);
            ServerInfo.SetUrl(ref CheckPhoneUrl);
        }

        public bool External;

        public void Next()
        {
            PageCheck();
        }

        public void NextPage()
        {
            WarningPanel.gameObject.SetActive(false);
            if (PageNum < 3)
            {
                content.SetTrigger("Next");
                PageNum++;
            }
        }

        public void Prev()
        {
            WarningPanel.gameObject.SetActive(false);
            if (PageNum == 0)
            {
                if (!LangPanel.activeInHierarchy)
                {
                    LangPanel.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                content.SetTrigger("Prev");
                PageNum--;
            }
        }

        void PageCheck()
        {
            _checks[PageNum]();
        }

        void CheckRegion()
        {
            if (Group.AnyTogglesOn())
            {
                NextPage();
            }
            else
            {
                CantContinue("regionchoose");
            }
        }

        void ComparePasswords()
        {
            var pass1 = Password.text;
            var pass2 = ConfirmPassword.text;

            Regex reg = new Regex(@"^(?=.*?[a-z])(?=.*?[0-9]).{6,20}$");
            if (reg.IsMatch(pass1))
            {
                if (pass1 == pass2)
                {
                    NewUser.Password = pass1;
                    NewUser.ConfirmPassword = pass2;
                    NewUser.RegionId = Region.Id;
                    LoginManager.Instance.RegisterAsync(NewUser);
                }
                else
                {
                    CantContinue(PasswordsMismatch);
                }
            }
            else
            {
                CantContinue(InvalidPassword);
            }
        }

        public void ChangePhoneCharacters(string input)
        {      
            Phone.text = _phoneNumberHandler.ChangePhoneCharacters(input, Region.PhonePlaceholder);
            Phone.MoveTextEnd(false);      
        }

        void CheckEmail()
        {
            var email = Email.text;
            _phone = Phone.text;
            if (string.IsNullOrEmpty(_phone))
            {
                CantContinue(InvalidFormatPhone);
                return;
            }
            else if (string.IsNullOrEmpty(email))
            {
                CantContinue(InvalidFormatEmail);
                return;
            }
            Regex reg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (reg.IsMatch(email))
            {
                bool exist;

                InputString parameters = new InputString() { Value = email };

                CoroutineManager.SendRequest(CheckEmailUrl, parameters,
                    preSuccessMethod: (response) =>
                    {
                        exist = bool.Parse(response.Text);
                        Debug.Log(exist);
                        if (exist)
                        {
                            CantContinue(EmailAlreadyExists);
                        }
                        else
                        {
                            CheckPhone();
                        }
                    });
            }
            else
                CantContinue(InvalidFormatEmail);
        }

        public void CheckPhone()
        {
            bool exist;
            InputString parameters = new InputString() { Value = _phone };

            if (Regex.IsMatch(_phone, Canvaser.Instance.RegistrationPanel.Region.PhonePattern))
            {
                CoroutineManager.SendRequest(CheckPhoneUrl, parameters,
                    preSuccessMethod: (response) =>
                    {
                        exist = bool.Parse(response.Text);
                        Debug.Log(exist);
                        if (exist)
                            CantContinue(PhoneAlreadyExists);
                        else
                        {
                            NewUser.Email = Email.text;
                            NewUser.PhoneNumber = _phone;
                            NextPage();
                        }
                    });
            }
            else
            {
                CantContinue(InvalidFormatPhone);
            }
        }

        void CheckNick()
        {
            var nick = Nick.text;
            if (!string.IsNullOrEmpty(nick))
            {
                bool exist;

                InputString parameters = new InputString() { Value = nick };
                CoroutineManager.SendRequest(CheckNickUrl, parameters,
                    preSuccessMethod: (response) =>
                    {
                        Debug.Log(response.Text);
                        exist = bool.Parse(response.Text);
                        if (exist)
                            CantContinue(NickAlreadyExists);
                        else
                        {
                            NewUser.Nickname = nick;
                            if (External)
                            {
                                NewUser.RegionId = Region.Id;
                                LoginManager.Instance.RegisterExternal(new RegisterExternalBindingModel(NewUser));
                            }
                            else
                            {
                                NextPage();
                            }
                        }
                    });
            }
            else
            {
                CantContinue(NickCantBeEmpty);
            }

        }

        void CantContinue(string message)
        {
            Debug.Log(message);
            message = LocalizationManager.GetLocalizedValue(message);
            WarningPanel.ShowMessage(message);
        }

        public void SetRegions(List<RegionModel> models)
        {
            Debug.Log(models.Count);
            ClearContent();
            regions = models;

            foreach (RegionModel item in regions)
            {
                RegionPanel panel = Instantiate(RegionObject, RegionsContent).GetComponent<RegionPanel>();
                panel.SetRegionPanel(item);
                panel.ToggleBox.group = Group;
            }
            Email.interactable = !External;
            gameObject.SetActive(true);
        }

        public void ClearContent()
        {
            RegionsContent.ClearContent();
            regions.Clear();
        }

        public void ExternalRegistration(string email)
        {
            External = true;
            Email.text = email;
            LoginManager.Instance.GetRegions();
        }

        public void InternalRegistration()
        {
            External = false;
            LoginManager.Instance.GetRegions();
            LangPanel.SetActive(true);
        }
    }
}
