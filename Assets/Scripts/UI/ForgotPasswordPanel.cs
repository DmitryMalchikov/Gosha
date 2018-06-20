using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordPanel : MonoBehaviour
{

    public GameObject EmailPanel;
    public GameObject PasswordPanel;
    public GameObject ResetFinishPanel;

    public InputField Email;
    public InputField Code;
    public InputField Password1;
    public InputField Password2;

    public GameObject WarningPanel;
    public Text WarningText;

    public void GetCode()
    {
        if (!string.IsNullOrEmpty(Email.text))
        {
            LoginManager.Instance.ForgotPasswordAsync(Email.text);
        }
        else
        {
            SetWarning(LocalizationManager.GetLocalizedValue("youneedtoenteremailfirst"));
        }
    }

    public void OpenPanel()
    {
        EmailPanel.SetActive(true);
        PasswordPanel.SetActive(true);
        PasswordPanel.SetActive(false);
        ResetFinishPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void OpenPasswordInputs()
    {
        if (!string.IsNullOrEmpty(Email.text))
        {
            if (Regex.IsMatch(Email.text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailPanel.SetActive(false);
                PasswordPanel.SetActive(true);
                ResetFinishPanel.SetActive(false);
            }
            else
            {
                SetWarning(LocalizationManager.GetLocalizedValue("invalidformatemail"));
            }
        }
        else
        {
            SetWarning(LocalizationManager.GetLocalizedValue("youneedtoenteremailfirst"));
        }
    }

    public void ResetFinished()
    {
        EmailPanel.SetActive(false);
        PasswordPanel.SetActive(false);
        ResetFinishPanel.SetActive(true);
    }

    public void ValidateCode()
    {
        LoginManager.Instance.ValidateResetTokenAsync(Email.text, Code.text);
    }

    public void ResetPassword()
    {
        Regex reg = new Regex(@"^(?=.*?[a-z])(?=.*?[0-9]).{6,20}$");
        if (reg.IsMatch(Password1.text))
        {
            if (Password1.text == Password2.text)
            {
                LoginManager.Instance.ResetPasswordAsync(Email.text, Password1.text, Password2.text, Code.text);
            }
            else
            {
                SetWarning(LocalizationManager.GetLocalizedValue("passwordsmismatch"));
            }
        }
        else
        {
            SetWarning(LocalizationManager.GetLocalizedValue("passworddoesnotcorrespondtherequirements"));
        }
    }

        public void SetWarning(string message)
        {
            StartCoroutine(WarningCloser(message));
        }

        public void ClearInputs()
        {
            Email.text = "";
            Code.text = "";
            Password1.text = "";
            Password2.text = "";
        }
        public IEnumerator WarningCloser(string message)
        {
            WarningText.text = message;
            WarningPanel.SetActive(true);
            yield return new WaitForSeconds(5f);
            WarningPanel.SetActive(false);
        }


        public void Finish()
        {
            ClearInputs();
            gameObject.SetActive(false);
        }
    }
