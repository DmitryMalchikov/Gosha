using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPrize : MonoBehaviour {
    
    public GameObject IceCream;
    public GameObject Freeze;
    public GameObject Magnet;
    public GameObject Shield;

    public Animator anim;

    GameObject ActiveObj;

    public void SetPrize(string name)
    {
        Debug.Log(name);
        switch(name)
        {
            case "Ice cream":
            case "Мороженое":
                IceCream.SetActive(true);
                ActiveObj = IceCream;
                break;
            case "Shield":
            case "Щит":
                Shield.SetActive(true);
                ActiveObj = Shield;
                break;
            case "Freeze":
            case "Заморозка":
                Freeze.SetActive(true);
                ActiveObj = Freeze;
                break;
            case "Magnet":
            case "Магнит":
                Magnet.SetActive(true);
                ActiveObj = Magnet;
                break;
            default:
                IceCream.SetActive(true);
                ActiveObj = IceCream;
                break;
                //ФИЛИПП!!! Вставь AudioManager.PlayCardGet() если выпала карточка
        }
    }

    public void PrizeOut(bool toOut)
    {
        anim.SetBool("Out",toOut);
    }

    public void TurnOffPrizes()
    {
        if(ActiveObj)
        ActiveObj.SetActive(false);
    }
}
