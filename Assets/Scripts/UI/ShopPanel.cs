using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public List<ItemInfo> Items;

    public Transform CaseContent;
    public GameObject CaseObject;
    public List<ItemInfo> Cases;

    public Toggle CardsTgl;
    public Toggle BonusesTgl;
    public Toggle CasesTgl;

    public void Open()
    {
        Synchroniser.NewSync(2);

        Synchroniser.OnActionsReady += CheckBuyBtns;
        //Synchroniser.OnActionsReady += () => gameObject.SetActive(true);
        gameObject.SetActive(true);
        LoginManager.Instance.GetUserInfoAsync(() => Synchroniser.SetReady(0));
        ShopManager.Instance.GetShopItemsAsync(() => Synchroniser.SetReady(1));
        if (LoginManager.LocalUser)
        {
            BonusesTgl.isOn = true;
            CardsTgl.interactable = false;
            CasesTgl.interactable = false;
        }
    }

    public void SetPromoBtn()
    {
        var date = LoginManager.User.BunnedUntil;

        if (date.HasValue && date > DateTime.Now)
        {
            PromosTxt.text = LocalizationManager.GetLocalizedValue("promocodebanned") + " " + (date.Value - DateTime.Now).IntervalToString();
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

    public void SetUpgrades(List<ShopItem> upgrades)
    {
        foreach (ShopItem item in upgrades)
        {
            ItemInfo current = Items.Find(x => x.ItemName == item.Name);
            if (LoginManager.LocalUser)
            {
                var upgrade = LoginManager.User.BonusUpgrades.Find(bu => bu.BonusName == item.Name);
                var amount = upgrade != null ? upgrade.UpgradeAmount : 0;
                current.Upgrade.value = amount;
                if (amount < 5)
                {
                    current.PriceText.text = (item.Cost * (amount + 1)).ToString();
                    current.BuyButton.interactable = true;
                }
                else
                {
                    current.BuyButton.interactable = false;
                    current.PriceText.text = "Max";
                }
            }
            else
            {
                current.Upgrade.value = item.Amount;
                if (item.Amount < 5)
                {
                    current.PriceText.text = (item.Cost * (item.Amount + 1)).ToString();
                    current.BuyButton.interactable = true;
                }
                else
                {
                    current.BuyButton.interactable = false;
                    current.PriceText.text = "Max";
                }
            }
            current.ItemID = item.Id;
        }
    }

    public void SetBonuses(List<ShopItem> upgrades)
    {
        foreach (ShopItem item in upgrades)
        {
            ItemInfo current = Items.Find(x => x.ItemName == item.Name);
            current.PriceText.text = item.Cost.ToString();
            current.ItemID = item.Id;
        }
    }

    public void CheckBuyBtns()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].BuyButton.interactable = Items[i].PriceText.text != "Max" && int.Parse(Items[i].PriceText.text) <= LoginManager.User.IceCream;
        }
        for (int i = 0; i < Cases.Count; i++)
        {
            Cases[i].BuyButton.interactable = int.Parse(Cases[i].PriceText.text) <= LoginManager.User.IceCream;
        }
        for (int i = 0; i < CardSets.Count; i++)
        {
            for (int j = 0; j < CardSets[i].Cards.Count; j++)
            {
                CardSets[i].Cards[j].BuyButton.interactable = int.Parse(CardSets[i].Cards[j].PriceText.text) <= LoginManager.User.IceCream;
            }
        }
    }

    public void SetCards(List<ShopCard> upgrades)
    {
        ClearCards();
        CardSetItem set;
        foreach (ShopCard item in upgrades)
        {
            set = CardSets.Find(x => x.SuitID == item.SuitId);
            if (set != null)
            {
                set.SetCard(item);
            }
            else
            {
                CardSetItem newSet = Instantiate(CardSetObject, CardContent).GetComponent<CardSetItem>();
                newSet.SetCardSet(item);
                CardSets.Add(newSet);
            }
        }
    }
    public void SetCases(List<ShopItem> upgrades)
    {
        ClearCases();
        foreach (ShopItem item in upgrades)
        {
            ItemInfo newCase = Instantiate(CaseObject, CaseContent).GetComponent<ItemInfo>();
            newCase.SetCase(item);
            Cases.Add(newCase);
        }
    }
    public void CleanContent(Transform content)
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
    }

    public void ClearCards()
    {
        CleanContent(CardContent);
        CardSets.Clear();
    }
    public void ClearCases()
    {
        CleanContent(CaseContent);
        Cases.Clear();
    }
}
