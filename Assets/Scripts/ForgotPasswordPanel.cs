using System.Collections;
using System.Collections.Generic;
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
        LoginManager.Instance.ForgotPasswordAsync(Email.text);
    }

    public void OpenPanel()
    {
        EmailPanel.SetActive(true);
        PasswordPanel.SetActive(true);
        PasswordPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void OpenPasswordInputs()
    {
        EmailPanel.SetActive(false);
        PasswordPanel.SetActive(true);
        ResetFinishPanel.SetActive(false);
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
        if (Password1.text == Password2.text)
        {
            LoginManager.Instance.ResetPasswordAsync(Email.text, Password1.text, Password2.text, Code.text);
        }
        else
        {
            SetWarning(LocalizationManager.GetLocalizedValue("passwordsmismatch"));
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
