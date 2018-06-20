using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuitsPanel : MonoBehaviour
{

    public SnapScrolling SuitsScroll;

    public GameObject CaseCamera;
    public GameObject SuitCamera;

    public Image CurrentSuitImage;
    public RawImage SuitImage;
    public RawImage CaseImage;

    public GameObject SuitPanel;
    public GameObject CardsPanel;

    public List<Image> Cards;

    public Button GetSuitBtn;
    public Button BuyCards;
    public Button ShowSuitsCards;
    public Button Share;
    public Button TakeOffSuitBtn;

    public Text PrizeText;
    public GameObject PrizePanel;

    public Text Warning;
    public Text SuitName;

    public Animator Case;
    public GameObject Idle;
    public GameObject PreopenCase;
    public GameObject OpenCaseParticle;

    public Button PutOnSuitBtn;

    public GameObject GetPrizePnl;
    public GameObject GetPrizeBtn;
    public Text PrizeAmountTxt;

    public BoxPrize BoxPrizeObj;

    public NewCardsPanel NewCards;

    public void SetPrize(Bonus bonus)
    {
        //PrizeText.text = string.Format("{0} {1}", bonus.Amount, bonus.Name);
        PrizeAmountTxt.text = LocalizationManager.GetLocalizedValue("yougot") + " " + ((LocalizationManager.CurrentLanguage == Language.EN) ? bonus.Name.Name : bonus.Name.NameRu);
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
        Case.SetBool("Open", true);
        PreopenCase.SetActive(false);
        OpenCaseParticle.SetActive(true);
        GetPrizeBtn.SetActive(true);
        if (!string.IsNullOrEmpty(PrizeAmountTxt.text))
        {
            PrizeAmountTxt.gameObject.SetActive(true);
        }
        BoxPrizeObj.PrizeOut(true);
    }

    public void TakePrize(bool toClose)
    {
        Case.SetBool("Open", false);
        OpenCaseParticle.SetActive(false);
        Idle.SetActive(true);
        PrizeAmountTxt.text = "";
        if (PrizeAmountTxt.gameObject.activeInHierarchy)
        {
            PrizeAmountTxt.gameObject.SetActive(false);
        }
        BoxPrizeObj.PrizeOut(false);
        BoxPrizeObj.TurnOffPrizes();
        GetPrizePnl.SetActive(false);
        GetPrizeBtn.SetActive(false);
        LoginManager.Instance.User.Cases--;
        Canvaser.Instance.CasesCount.text = ": " + LoginManager.Instance.User.Cases;

        LoginManager.Instance.GetUserInfoAsync(() =>
        {
            if (!toClose)
            {
                InventoryManager.Instance.GetMyCasesAsync();
            }
        });
    }

    public void SetCurrentCostume(Costume suit)
    {
        string Name = suit.Name;
        bool HasSuit = suit.CostumeAmount > 0;
        PutOnSuitBtn.gameObject.SetActive(HasSuit);

        TakeOffSuitBtn.gameObject.SetActive(PlayerPrefs.GetString("CurrentSuit") == suit.Name);

        if (Name == "FB Suit" || Name == "OK Suit" || Name == "VK Suit" || Name == "Unicorn Suit")
        {
            ShowSuitsCards.gameObject.SetActive(false);
            Share.gameObject.SetActive(!HasSuit);
            NewCards.Close();
        }
        else
        {
            Share.gameObject.SetActive(false);
            if (HasSuit)
            {
                NewCards.Open(suit);
            }
            else
            {
                NewCards.Close();
            }
            ShowSuitsCards.gameObject.SetActive(true);
            UpdateCards();
        }
        //CurrentSuitImage.sprite = selected.sprite;
        PlayerController.Instance.PutOnSuit(Name);
        SuitName.text = Name;
    }

    public void PutOnSuit()
    {
        SuitsScroll.PutOnSuit();
        PlayerPrefs.SetString("CurrentSuit", SuitName.text);
    }

    public void TakeOffSuit()
    {
        SuitsScroll.PutOnSuit(false);
        PlayerPrefs.SetString("CurrentSuit", "");
    }

    public void SetCurrentCase(Image selected)
    {
        //CurrentSuitImage.texture = selected.sprite.texture;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Canvaser.Instance.Suits.ResetPanel();
        CardsPanel.SetActive(false);
        SuitPanel.SetActive(true);

        InventoryManager.Instance.GetSuitsAsync();
    }


    public void SetCostumes(List<Costume> costumes)
    {
        Debug.Log(costumes.Count);
        if (costumes.Count == 0)
        {
            SuitImage.gameObject.SetActive(false);
            Warning.text = LocalizationManager.GetLocalizedValue("nosuits");
            Warning.gameObject.SetActive(true);
        }
        else
        {
            SuitCamera.SetActive(true);
            SuitImage.gameObject.SetActive(true);
            Warning.gameObject.SetActive(false);
        }

        //SuitsScroll.SetCostumes(costumes);
        SuitsScroll.gameObject.SetActive(true);
        SuitsScroll.SetCostumes(costumes);
    }

    public void ResetPanel()
    {
        SuitsScroll.gameObject.SetActive(false);
        SuitCamera.SetActive(false);
        SuitImage.gameObject.SetActive(false);
        Warning.gameObject.SetActive(false);
    }

    public void ShowCards()
    {
        SuitPanel.SetActive(false);
        PlayerController.Instance.PutOnSuit(PlayerPrefs.GetString("CurrentSuit"));
        CardsPanel.SetActive(true);
        UpdateCards();
    }

    public void TurnOffCards()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i].gameObject.SetActive(false);
        }
    }

    public void SetCard(InventoryCard card, string SuitName)
    {
        //set card image by costume ID
        Cards[card.Position - 1].gameObject.SetActive(card.Amount > 0);
        Cards[card.Position - 1].GetComponent<Image>().sprite = Resources.Load<Sprite>(SuitName + " (" + card.Position + ")");
    }

    public void UpdateCards()
    {
        Debug.Log("Update cards");
        TurnOffCards();
        if (SuitsScroll.Costumes.Count > 0)
        {
            foreach (InventoryCard item in SuitsScroll.Costumes[SuitsScroll.selectedPanID].Cards)
            {
                SetCard(item, SuitsScroll.Costumes[SuitsScroll.selectedPanID].Name);
            }
        }

        if (Cards.FindAll(x => x.gameObject.activeInHierarchy).Count == 4)
        {
            BuyCards.gameObject.SetActive(false);
            GetSuitBtn.gameObject.SetActive(true);
        }
        else
        {
            BuyCards.gameObject.SetActive(true);
            GetSuitBtn.gameObject.SetActive(false);
        }
    }

    public void ShowCostume()
    {
        CardsPanel.SetActive(false);
        PlayerController.Instance.PutOnSuit(SuitName.text);
        SuitPanel.SetActive(true);
    }


    public void ClosePanel()
    {
        if (Canvaser.Instance.CasesPanel.gameObject.activeInHierarchy)
        {
            CaseCamera.SetActive(false);
        }
        else
        {
            PlayerController.Instance.PutOnSuit(PlayerPrefs.GetString("CurrentSuit"));
            SuitCamera.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void GetSuit()
    {
        ShopManager.Instance.GetSuitAsync(SuitsScroll.Costumes[SuitsScroll.selectedPanID].CostumeId);
    }

    public void SetCases(List<InventoryItem> cases)
    {
        Debug.Log(cases.Count);
        if (cases.Count == 0)
        {
            CaseImage.gameObject.SetActive(false);
            Warning.text = LocalizationManager.GetLocalizedValue("nocases");
            Warning.gameObject.SetActive(true);
        }
        else
        {
            CaseCamera.SetActive(true);
            CaseImage.gameObject.SetActive(true);
            Warning.gameObject.SetActive(false);
        }
        //SuitsScroll.SetCases(cases);
        gameObject.SetActive(true);
    }
    public void OpenCase()
    {
        GetPrizePnl.SetActive(true);
        //LootManager.Instance.OpenCaseAsync(SuitsScroll.Cases[SuitsScroll.selectedPanID].Id);
        PreopenCase.SetActive(true);
        Idle.SetActive(false);
        LootManager.Instance.OpenCaseAsync(LoginManager.Instance.User.CaseId);
        AudioManager.PlayCaseOpen();
    }
}
