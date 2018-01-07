using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RegisterBindingModel
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    public string PhoneNumber { get; set; }

    public string Nickname { get; set; }

    public int RegionId { get; set; }
}

public class Registration : MonoBehaviour
{

    public string CheckEmailUrl = "/api/account/checkemail";
    public string CheckNickUrl = "/api/account/checknickname";
    public string CheckPhoneUrl = "/api/account/checkphone";

    public Animator pages;
    public Animator content;

    public int PageNum = 1;

    public InputField Email;
    public InputField Phone;
    public InputField Nick;
    public InputField Password;
    public InputField ConfirmPassword;

    public bool isRUS { get; set; }

    bool canContinue = true;

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

    public void Start()
    {
        SetUrls();
    }

    public void SetUrls()
    {
        CheckEmailUrl = ServerInfo.GetUrl(CheckEmailUrl);
        CheckNickUrl = ServerInfo.GetUrl(CheckNickUrl);
        CheckPhoneUrl = ServerInfo.GetUrl(CheckPhoneUrl);
    }

    public bool External;

    public void Next()
    {
        PageCheck();
        
    }

    public void NextPage()
    {
        if (!(PageNum == 4))
        {
            content.SetTrigger("Next");
            PageNum++;
        }
    }

    public void Prev()
    {
        if (PageNum == 1)
        {
            WarningPanel.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            content.SetTrigger("Prev");
            PageNum--;
        }
    }

    void PageCheck()
    {
        switch (PageNum)
        {
            case 1:
                NextPage();
                break;
            case 2:
                CheckEmail(Email.text);
                break;
            case 3:
                CheckNick(Nick.text);
                break;
            case 4:

                ComparePasswords(Password.text, ConfirmPassword.text);
                break;
        }
    }

    void ComparePasswords(string pass1, string pass2)
    {
        Debug.Log(pass1 + "\n" + pass2);
        Regex reg = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-\.]).{4,20}$");
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

    void CheckEmail(string email)
    {
        if(string.IsNullOrEmpty(Phone.text))
        {
            CantContinue(InvalidFormatPhone);
            return;
        }
        else if(string.IsNullOrEmpty(email))
        {
            CantContinue(InvalidFormatEmail);
            return;
        }
        Regex reg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        if(reg.IsMatch(email))
        { 
            bool exist;

            InputString parameters = new InputString() { Value = email };

            StartCoroutine(NetworkHelper.SendRequest(CheckEmailUrl, JsonConvert.SerializeObject(parameters), "application/json", (response) =>
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
            }));
        }
    else
            CantContinue(InvalidFormatEmail);
    }

    public void CheckPhone()
    {
        bool exist;
        InputString parameters = new InputString() { Value = Phone.text };

        StartCoroutine(NetworkHelper.SendRequest(CheckPhoneUrl, JsonConvert.SerializeObject(parameters), "application/json", (response) =>
        {
            exist = bool.Parse(response.Text);
            Debug.Log(exist);
            if (exist)
                CantContinue(PhoneAlreadyExists);
            else
            {
                NewUser.Email = Email.text;
                NewUser.PhoneNumber = Phone.text;
                NextPage();
            }
        }));
     }

    void CheckNick(string nick)
    {
        bool exist;

        InputString parameters = new InputString() { Value = nick };
        StartCoroutine(NetworkHelper.SendRequest(CheckNickUrl, JsonConvert.SerializeObject(parameters), "application/json", (response) =>
        {

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
        }));
    }

    void CantContinue(string message)
    {
        canContinue = false;
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
        gameObject.SetActive(true);
    }

    public void ClearContent()
    {
        foreach ( Transform item in RegionsContent)
        {
            Destroy(item.gameObject);
        }
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
    }
}
