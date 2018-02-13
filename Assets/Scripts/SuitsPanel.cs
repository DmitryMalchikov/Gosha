﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuitsPanel : MonoBehaviour {

    public SnapScrolling SuitsScroll;

    public Image CurrentSuitImage;
    public RawImage CaseImage;

    public GameObject SuitPanel;
    public GameObject CardsPanel;

    public List<Image> Cards;

    public Button GetSuitBtn;
    public Button BuyCards;
    public Button ShowSuitsCards;

    public Text PrizeText;
    public GameObject PrizePanel;

    public Text Warning;
    public Text SuitName;

    public Animator Case;
    public GameObject Idle;
    public GameObject PreopenCase;
    public GameObject OpenCaseParticle;
    

    public void SetPrize(Bonus bonus)
    {
        PrizeText.text = string.Format("{0} {1}", bonus.Amount, bonus.Name);
        Case.SetBool("Open", true);
        PreopenCase.SetActive(false);
        OpenCaseParticle.SetActive(true);
        StartCoroutine(WaitForOpen());
    }

    IEnumerator WaitForOpen()
    {
        yield return new WaitForSeconds(1f);
        PrizePanel.SetActive(true);
        Case.SetBool("Open", false);
        OpenCaseParticle.SetActive(false);
        Idle.SetActive(true);
    }

    public void SetCurrentCostume(Image selected, string Name)
    {
        if(Name == "FB Suit" || Name == "OK Suit" || Name == "VK Suit")
        {
            ShowSuitsCards.gameObject.SetActive(false);
        }
        else
        {
            ShowSuitsCards.gameObject.SetActive(true);
            UpdateCards();
        }
        CurrentSuitImage.sprite = selected.sprite;
        SuitName.text = Name;
    }

    public void SetCurrentCase(Image selected)
    {
        //CurrentSuitImage.texture = selected.sprite.texture;
    }

    public void Open()
    {
        Canvaser.ShowLoading(true);
        CardsPanel.SetActive(false);
        SuitPanel.SetActive(true);
        InventoryManager.Instance.GetSuitsAsync();
    }
    

    public void SetCostumes(List<Costume> costumes)
    {
        Debug.Log(costumes.Count);
        if (costumes.Count == 0)
        {
            CurrentSuitImage.gameObject.SetActive(false);
            Warning.text = LocalizationManager.GetLocalizedValue("nosuits");
            Warning.gameObject.SetActive(true);
        }
        else
        {
            CurrentSuitImage.gameObject.SetActive(true);
            Warning.gameObject.SetActive(false);
        }
        gameObject.SetActive(true);
        Canvaser.ShowLoading(false);
        SuitsScroll.SetCostumes(costumes);
        //UpdateCards();
    }

    public void ShowCards()
    {
        SuitPanel.SetActive(false);
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

        if(Cards.FindAll(x => x.gameObject.activeInHierarchy).Count == 4)
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
        SuitPanel.SetActive(true);
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
            CaseImage.gameObject.SetActive(true);
            Warning.gameObject.SetActive(false);
        }
        SuitsScroll.SetCases(cases);
        Canvaser.ShowLoading(false);
        gameObject.SetActive(true);
    }
    public void OpenCase()
    {
        //LootManager.Instance.OpenCaseAsync(SuitsScroll.Cases[SuitsScroll.selectedPanID].Id);
        PreopenCase.SetActive(true);
        Idle.SetActive(false);
        LootManager.Instance.OpenCaseAsync(SuitsScroll.CasesIds[SuitsScroll.selectedPanID]);
    }
}
