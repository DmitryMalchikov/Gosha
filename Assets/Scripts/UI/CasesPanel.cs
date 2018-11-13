using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CasesPanel : MonoBehaviour
{
    public GameObject CaseCamera;
    public RawImage CaseImage;
    public GameObject SuitPanel;
    public Text PrizeText;
    public GameObject PrizePanel;
    public Text Warning;
    public Text SuitName;
    public Animator Case;
    public GameObject Idle;
    public GameObject PreopenCase;
    public GameObject OpenCaseParticle;
    public GameObject GetPrizePnl;
    public GameObject GetPrizeBtn;
    public Text PrizeAmountTxt;
    public BoxPrize BoxPrizeObj;
    public ParticleEmitter SparksAnimator;

    public void SetPrize(Bonus bonus)
    {
        PrizeAmountTxt.text = LocalizationManager.GetLocalizedValue("yougot") + " " + LocalizationManager.GetValue(bonus.Name.NameRu, bonus.Name.Name);
        if (bonus.Amount > 1)
        {
            PrizeAmountTxt.text += "(" + bonus.Amount.ToString() + ")";
        }
        BoxPrizeObj.SetPrize(bonus.Name.Name);
        StartCoroutine(WaitForOpen());
    }

    IEnumerator WaitForOpen()
    {
        yield return new WaitForSeconds(3f);
        OpenCase(true);
        PreopenCase.SetActive(false);
        if (!string.IsNullOrEmpty(PrizeAmountTxt.text))
        {
            PrizeAmountTxt.gameObject.SetActive(true);
        }

        SparksAnimator.emit = true;
        yield return new WaitForSeconds(0.5f);
        SparksAnimator.emit = false;
    }

    public void TakePrize()
    {
        LoginManager.User.Cases--;
        Canvaser.Instance.CasesCount.text = ": " + LoginManager.User.Cases.ToString();
        CloseCase();
        LoginManager.Instance.GetUserInfoAsync(() =>
        {
            InventoryManager.Instance.GetMyCasesAsync();
        });
    }

    public void OpenCase(bool open)
    {
        Case.SetBool("Open", open);
        OpenCaseParticle.SetActive(open);
        GetPrizeBtn.SetActive(open);
        BoxPrizeObj.PrizeOut(open);
    }

    public void CloseCase()
    {
        OpenCase(false);
        Idle.SetActive(true);
        PrizeAmountTxt.text = "";
        if (PrizeAmountTxt.gameObject.activeInHierarchy)
        {
            PrizeAmountTxt.gameObject.SetActive(false);
        }
        BoxPrizeObj.TurnOffPrizes();
        GetPrizePnl.SetActive(false);       
    }

    public void ClosePanel()
    {
        CloseCase();
        CaseCamera.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SetCases(List<InventoryItem> cases)
    {
        if (cases.Count < 1 || cases[0].Amount < 1)
        {
            Warning.text = LocalizationManager.GetLocalizedValue("nocases");
            ShowWarning(true);
        }
        else
        {
            CaseCamera.SetActive(true);
            ShowWarning(false);
        }
        gameObject.SetActive(true);
    }

    public void OpenCase()
    {
        if (LoginManager.User.Cases < 1)
        {
            return;
        }

        GetPrizePnl.SetActive(true);
        PreopenCase.SetActive(true);
        Idle.SetActive(false);
        LootManager.Instance.OpenCaseAsync(LoginManager.User.CaseId);
        AudioManager.PlayCaseOpen();
    }

    public void ShowWarning(bool show)
    {
        Warning.gameObject.SetActive(show);
        CaseImage.gameObject.SetActive(!show);
    }
}
