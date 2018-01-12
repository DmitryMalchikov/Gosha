using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IPickable
{
    
    public virtual void PickUp()
    {       
        if (GameController.Instance.Magnet)
        {
            StartCoroutine(MoveToPlayer());
        }
        else
        {
            GameController.Instance.AddCoin();
            gameObject.SetActive(false);         
        }
    }

    IEnumerator MoveToPlayer()
    {
        float startSpeed = 0;

        //while (Vector3.Distance(transform.position, PlayerController.Instance.transform.position + Vector3.forward / 2) > 0.5f)
        while (true)
        {
            yield return GameController.Frame;
            transform.position = Vector3.MoveTowards(transform.position, PlayerController.Instance.transform.position + Vector3.forward / 2, startSpeed);
            startSpeed += GameController.Instance.CoinSpeed * GameController.Instance.Speed.z;
        }
        
        //GameController.Instance.AddCoin();
        //gameObject.SetActive(false);
    }
}
