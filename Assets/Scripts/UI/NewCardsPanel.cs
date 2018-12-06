using Assets.Scripts.DTO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class NewCardsPanel : MonoBehaviour
    {
        public Image[] Images;
        public Text[] Counters;

        public void Open(Costume suit)
        {
            for (int i = 0; i < Images.Length; i++)
            {
                Images[i].sprite = Resources.Load<Sprite>(suit.Name + " (" + (i + 1) + ")");
                Counters[i].text = suit.Cards[i].Amount.ToString();
            }
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

    }
}
