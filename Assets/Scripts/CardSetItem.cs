using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSetItem : MonoBehaviour {

    public int SuitID;
    public Text Title;
    public List<ItemInfo> Cards;
	public Image SuitImage;

    public void SetCardSet(ShopCard card)
    {
        SuitID = card.SuitId;
        Title.text = LocalizationManager.GetLocalizedValue("suitcards") + LocalizationManager.GetValue(card.NameRu, card.Name);
		SuitImage.sprite = Resources.Load<Sprite> (card.SuitName);
        SetCard(card);
    }
    public void SetCard(ShopCard card)
    {
        Cards[card.Position -1].SetCard(card);
    }
}
