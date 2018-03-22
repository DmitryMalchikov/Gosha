using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
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

    string phone;

    public GameObject LangPanel;

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
        switch (PageNum)
        {
            case 1:
                CheckRegion();
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

    void CheckRegion()
    {
        if(Group.AnyTogglesOn())
        {
            NextPage();
        }
        else
        {
            CantContinue(LocalizationManager.GetLocalizedValue("regionchoose"));
        }
    }

    void ComparePasswords(string pass1, string pass2)
    {
        Debug.Log(pass1 + "\n" + pass2);
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

	private int _previousLength = 0;
	private string _previousInput;
	private string _result = string.Empty;

    public void ChangePhoneCharacters(string input)
    {
		if (input == _result) {
			return;
		}
		bool isDeleting = _previousLength > input.Length;
		char deletedCharacter = ' ';
		_previousLength = input.Length;
		if (isDeleting && !string.IsNullOrEmpty(_previousInput)) {
			deletedCharacter = _previousInput [_previousInput.Length - 1];
		}
		_previousInput = input;

		char defaultCharacter = '_';
		string charactersType = @"\d";
		char placeholder = '#';
		string template = Region.PhonePlaceholder.Replace (defaultCharacter, placeholder);

        StringBuilder builder = new StringBuilder(template);
        Regex reg = new Regex(charactersType);
        var matches = reg.Matches(input);
		int matchesCount = matches.Count;

        int index = -1;

        for (int i = 0; i < matches.Count; i++)
        {
            index = -1;
            for (int j = 0; j < builder.Length; j++)
            {
                if (builder[j] == placeholder)
                {
                    index = j;
                    break;
                }
            }

            if (index > -1)
            {
                builder.Replace(placeholder, matches[i].Value[0], index, 1);
            }
        }
			

		for (int i = builder.Length - 1; builder.Length > 0 && i >= 0 ; i--) {
			if (builder [i]== placeholder) {
				builder.Remove (i, 1);
			} else if(!Regex.IsMatch(builder[i].ToString(), charactersType))
			{
				if (i > 0) {
					if (isDeleting) {
						if (!Regex.IsMatch (builder [i].ToString (), charactersType)) {
							if (Regex.IsMatch (builder [i - 1].ToString (), charactersType)) {
								if (!Regex.IsMatch (deletedCharacter.ToString (), charactersType)) {
									builder.Remove (i - 1, 2);
									i--;
								} else {
									break;
								}
							} else {
								builder.Remove (i, 1);
							}
						} else {
							builder.Remove (i, 1);
						}
					} else if (Regex.IsMatch (builder [i - 1].ToString (), charactersType)) {
						break;
					} else {
						builder.Remove (i, 1);
					}
				} else {
					builder.Remove (i, 1);
				}
			}
				else {
				break;
			}
		}

        Phone.text = builder.ToString();
		Phone.MoveTextEnd (false);
		_result = builder.ToString ();
    }

    public string GetPhoneNumbers(string phone)
    {
        string res = phone;
        string pattern = "+()-";
        for (int i = 0; i < pattern.Length; i++)
        {
            res = res.Replace(pattern[i].ToString(),"");
        }
        return res;
    }

    void CheckEmail(string email)
    {
        phone = Phone.text;
        if(string.IsNullOrEmpty(phone))
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
        InputString parameters = new InputString() { Value = phone };

		if (Regex.IsMatch (phone, Canvaser.Instance.RegistrationPanel.Region.PhonePattern)) {
			StartCoroutine (NetworkHelper.SendRequest (CheckPhoneUrl, JsonConvert.SerializeObject (parameters), "application/json", (response) => {
				exist = bool.Parse (response.Text);
				Debug.Log (exist);
				if (exist)
					CantContinue (PhoneAlreadyExists);
				else {
					NewUser.Email = Email.text;
					NewUser.PhoneNumber = phone;
					NextPage ();
				}
			}));
		} else {
			CantContinue(InvalidFormatPhone);
		}
     }

    void CheckNick(string nick)
    {
        bool exist;

        InputString parameters = new InputString() { Value = nick };
        StartCoroutine(NetworkHelper.SendRequest(CheckNickUrl, JsonConvert.SerializeObject(parameters), "application/json", (response) =>
        {

            exist = bool.Parse(response.Text);
            if (exist)
                CantContinue(LocalizationManager.GetLocalizedValue("nickalreadyexists"));
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
        Debug.Log(message);
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
        LangPanel.SetActive(true);
    }
}
