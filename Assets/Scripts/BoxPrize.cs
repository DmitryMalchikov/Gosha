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
                IceCream.SetActive(true);
                ActiveObj = IceCream;
                break;
            case "Shield":
                Shield.SetActive(true);
                ActiveObj = Shield;
                break;
            case "Freeze":
                Freeze.SetActive(true);
                ActiveObj = Freeze;
                break;
            case "Magnet":
                Magnet.SetActive(true);
                ActiveObj = Magnet;
                break;
            case "Мороженое":
                IceCream.SetActive(true);
                ActiveObj = IceCream;
                break;
            case "Щит":
                Shield.SetActive(true);
                ActiveObj = Shield;
                break;
            case "Заморозка":
                Freeze.SetActive(true);
                ActiveObj = Freeze;
                break;
            case "Магнит":
                Magnet.SetActive(true);
                ActiveObj = Magnet;
                break;
            default:
                IceCream.SetActive(true);
                ActiveObj = IceCream;
                break;

        }
    }

    public void PrizeOut()
    {
        anim.SetTrigger("Out");
    }

    public void TurnOffPrizes()
    {
        ActiveObj.SetActive(false);
    }
}
