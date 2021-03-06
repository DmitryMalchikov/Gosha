﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPrize : MonoBehaviour {
    
    public GameObject IceCream;
    public GameObject Freeze;
    public GameObject Magnet;
    public GameObject Shield;
    public PrizeCard Card;

    public Animator anim;

    GameObject ActiveObj;

    string currentName;

    public void SetPrize(string name)
    {
        currentName = name;
        switch(name)
        {
            case "IceCream":
            case null:
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
            default:
                Card.SetCard(Canvaser.Instance.AddBrackets(name));
                ActiveObj = Card.gameObject;
                break;
        }
    }

    public void PrizeOut(bool toOut)
    {
        anim.SetBool("Out",toOut);
        if(toOut && !string.IsNullOrEmpty(currentName) && currentName.Contains("Card"))
        {
            AudioManager.PlayCardGet();
        }
    }

    public void TurnOffPrizes()
    {
        if(ActiveObj)
        ActiveObj.SetActive(false);
    }
}
