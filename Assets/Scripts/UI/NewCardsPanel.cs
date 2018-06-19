using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCardsPanel : MonoBehaviour {

    public List<Image> Images;
    public List<Text> Counters;

    public void Open(Costume suit)
    {
        for (int i = 0; i < Images.Count; i++)
        {
            Images[i].sprite = Resources.Load<Sprite>(suit.Name + " (" + (i+1) + ")");
            Counters[i].text = suit.Cards[i].Amount.ToString();
        }
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

}
