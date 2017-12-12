using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSetItem : MonoBehaviour {

    public int SuitID;
    public Text Title;
    public List<ItemInfo> Cards;

    public void SetCardSet(ShopCard card)
    {
        SuitID = card.SuitId;
        Title.text = LocalizationManager.GetLocalizedValue("suitcards") + card.SuitName;
        SetCard(card);
    }
    public void SetCard(ShopCard card)
    {
        Cards[card.Position -1].SetCard(card);
    }
}
