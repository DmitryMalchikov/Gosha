using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ShopPanel : MonoBehaviour
    {
        public Button PromosBtn;
        public Text PromosTxt;
        public Text IceCream;
        public GameObject EnterPromoPanel;
        public InputField PromoInput;
        public GameObject PromoAnswerPanel;
        public Text PromoAnswerText;
        public Transform CardContent;
        public GameObject CardSetObject;
        public List<CardSetItem> CardSets;
        public ItemInfo[] Items;

        public Transform CaseContent;
        public GameObject CaseObject;
        public List<ItemInfo> Cases;

        public Toggle CardsTgl;
        public Toggle BonusesTgl;
        public Toggle CasesTgl;

        private List<CustomTask> _currentTasks = new List<CustomTask>();

        public void Open()
        {
            gameObject.SetActive(true);

            RequestShopData();

            if (LoginManager.LocalUser)
            {
                BonusesTgl.isOn = true;
            }
            else
            {
                CardsTgl.isOn = true;
            }
            CardsTgl.interactable = !LoginManager.LocalUser;
            CasesTgl.interactable = !LoginManager.LocalUser;
        }

        public void RequestShopData()
        {
            _currentTasks.Clear();
            _currentTasks.Add(LoginManager.Instance.GetUserInfoAsync());
            _currentTasks.Add(ShopManager.Instance.GetShopItemsAsync());
            StartCoroutine(WaitTasks(CheckBuyBtns));
        }

        public void SetPromoBtn()
        {
            var date = LoginManager.User.BunnedUntil;

            if (date.HasValue && date > DateTime.Now)
            {
                PromosTxt.text = LocalizationManager.GetLocalizedValue("promocodebanned") + " " + (date.Value - DateTime.Now).TimeSpanToLocalizedString();
                PromosBtn.interactable = false;
            }
            else
            {
                PromosTxt.text = LocalizationManager.GetLocalizedValue("usepromocode");
                PromosBtn.interactable = true;
            }
        }

        public void UpdatePanel(int iceCream)
        {
            IceCream.text = iceCream.ToString();

        }
        public void EnterPromo()
        {
            ShopManager.Instance.EnterPromoCodeAsync(PromoInput.text);
        }

        public void PromoAnswer(string iceCreamCount)
        {
            if (int.Parse(iceCreamCount) == 0)
            {
                PromoAnswerText.text = LocalizationManager.GetLocalizedValue("promonotvalid");
            }
            else
            {
                PromoAnswerText.text = string.Format(LocalizationManager.GetLocalizedValue("earnedicecream"), iceCreamCount);
            }
            PromoInput.text = "";
            PromoAnswerPanel.SetActive(true);
            EnterPromoPanel.SetActive(false);
        }

        public void SetUpgrades(ShopItem[] upgrades)
        {
            for (int i = 0; i < upgrades.Length; i++)
            {
                var item = upgrades[i];
                ItemInfo current = Items.FirstOrDefault(x => x.ItemName == item.Name);
                current.SetUpgrade(item);
            }
        }

        public void SetBonuses(ShopItem[] upgrades)
        {
            for (int i = 0; i < upgrades.Length; i++)
            {
                ItemInfo current = Items.FirstOrDefault(x => x.ItemName == upgrades[i].Name);
                current.SetBonus(upgrades[i]);
            }
        }

        public void CheckBuyBtns()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i].BuyButton.interactable = Items[i].PriceText.text != "Max" && Items[i].Price <= LoginManager.User.IceCream;
            }
            for (int i = 0; i < Cases.Count; i++)
            {
                Cases[i].BuyButton.interactable = Cases[i].Price <= LoginManager.User.IceCream;
            }
            for (int i = 0; i < CardSets.Count; i++)
            {
                for (int j = 0; j < CardSets[i].Cards.Length; j++)
                {
                    CardSets[i].Cards[j].BuyButton.interactable = CardSets[i].Cards[j].Price <= LoginManager.User.IceCream;
                }
            }
        }

        public void SetCards(ShopCard[] cards)
        {
            ClearCards();
            for (int i = 0; i < cards.Length; i++)
            {
                var set = CardSets.Find(x => x.SuitID == cards[i].SuitId);
                if (set != null)
                {
                    set.SetCard(cards[i]);
                }
                else
                {
                    CardSetItem newSet = Instantiate(CardSetObject, CardContent).GetComponent<CardSetItem>();
                    newSet.SetCardSet(cards[i]);
                    CardSets.Add(newSet);
                }
            }
        }
        public void SetCases(ShopItem[] cases)
        {
            ClearCases();
            for (int i = 0; i < cases.Length; i++)
            {
                ItemInfo newCase = Instantiate(CaseObject, CaseContent).GetComponent<ItemInfo>();
                newCase.SetCase(cases[i]);
                Cases.Add(newCase);
            }
        }

        public void ClearCards()
        {
            CardContent.ClearContent();
            CardSets.Clear();
        }
        public void ClearCases()
        {
            CaseContent.ClearContent();
            Cases.Clear();
        }

        private IEnumerator WaitTasks(Action successAction)
        {
            yield return  new WaitUntil(() => CustomTask.TasksReady(_currentTasks));
            successAction();
            _currentTasks.Clear();
        }
    }
}
