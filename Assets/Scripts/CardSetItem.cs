using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CardSetItem : MonoBehaviour
    {
        public int SuitID;
        public Text Title;
        public ItemInfo[] Cards;
        public Image SuitImage;

        public void SetCardSet(ShopCard card)
        {
            SuitID = card.SuitId;
            Title.text = LocalizationManager.GetLocalizedValue("suitcards") + LocalizationManager.GetValue(card.SuitNameRu, card.SuitName);
            SuitImage.sprite = Resources.Load<Sprite>(card.SuitName);
            SetCard(card);
        }
        public void SetCard(ShopCard card)
        {
            Cards[card.Position - 1].SetCard(card);
        }
    }
}
