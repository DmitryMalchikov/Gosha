﻿using System.Linq;
using Assets.Scripts.DTO;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SuitsPanel : MonoBehaviour
    {
        public SnapScrolling SuitsScroll;
        public GameObject SuitCamera;
        public Image CurrentSuitImage;
        public RawImage SuitImage;
        public GameObject SuitPanel;
        public GameObject CardsPanel;
        public Image[] Cards;
        public Button GetSuitBtn;
        public Button BuyCards;
        public Button ShowSuitsCards;
        public Button Share;
        public Button TakeOffSuitBtn;
        public Text Warning;
        public Text SuitName;
        public Button PutOnSuitBtn;
        public NewCardsPanel NewCards;

        private string _currentSuitName;

        public void SetCurrentCostume(Costume suit)
        {
            string Name = LocalizationManager.GetValue(suit.NameRu, suit.Name);
            bool HasSuit = suit.CostumeAmount > 0;
            PutOnSuitBtn.gameObject.SetActive(HasSuit);

            TakeOffSuitBtn.gameObject.SetActive(PlayerPrefs.GetString("CurrentSuit") == suit.Name);

            if (suit.Cards.Length == 0)
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
            SuitsManager.PutOnSuit(suit.Name);
            SuitName.text = Name;
            _currentSuitName = suit.Name;
        }

        public void PutOnSuit()
        {
            SuitsScroll.PutOnSuit();
            PlayerPrefs.SetString("CurrentSuit", _currentSuitName);
        }

        public void TakeOffSuit()
        {
            SuitsScroll.PutOnSuit(false);
            PlayerPrefs.SetString("CurrentSuit", "");
        }

        public void Open()
        {
            if (Canvaser.Instance.IsLoggedIn())
            {
                gameObject.SetActive(true);
                Canvaser.Instance.Suits.ResetPanel();
                CardsPanel.SetActive(false);
                SuitPanel.SetActive(true);

                InventoryManager.Instance.GetSuitsAsync();
            }
        }

        public void OpenWithForceUpdate()
        {
            gameObject.SetActive(true);
            Canvaser.Instance.Suits.ResetPanel();
            CardsPanel.SetActive(false);
            SuitPanel.SetActive(true);

            InventoryManager.Instance.GetSuitsAsync(true);
        }

        public void SetCostumes(Costume[] costumes)
        {
            if (costumes.Length == 0)
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
            SuitsManager.PutOnSuit(PlayerPrefs.GetString("CurrentSuit"));
            CardsPanel.SetActive(true);
            UpdateCards();
        }

        public void TurnOffCards()
        {
            for (int i = 0; i < Cards.Length; i++)
            {
                Cards[i].gameObject.SetActive(false);
            }
        }

        public void SetCard(InventoryCard card, string SuitName)
        {
            Cards[card.Position - 1].gameObject.SetActive(card.Amount > 0);
            Cards[card.Position - 1].GetComponent<Image>().sprite = Resources.Load<Sprite>(SuitName + " (" + card.Position + ")");
        }

        public void UpdateCards()
        {
            TurnOffCards();
            if (SuitsScroll.Costumes.Length > 0)
            {
                foreach (InventoryCard item in SuitsScroll.Costumes[SuitsScroll.selectedPanID].Cards)
                {
                    SetCard(item, SuitsScroll.Costumes[SuitsScroll.selectedPanID].Name);
                }
            }

            if (Cards.Count(x => x.gameObject.activeInHierarchy) == 4)
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
            SuitsManager.PutOnSuit(SuitName.text);
            SuitPanel.SetActive(true);
        }

        public void ClosePanel()
        {
            SuitsManager.PutOnSuit(PlayerPrefs.GetString("CurrentSuit"));
            SuitCamera.SetActive(false);
            gameObject.SetActive(false);
        }

        public void GetSuit()
        {
            ShopManager.Instance.GetSuitAsync(SuitsScroll.Costumes[SuitsScroll.selectedPanID].CostumeId);
        }
    }
}
