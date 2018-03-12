using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IPickable
{
    
    public virtual void PickUp()
    {   
      	GameController.Instance.AddCoin();
      	gameObject.SetActive(false);         
    }

	public void OnMagnet(){
		StartCoroutine (MoveToPlayer ());
	}

    IEnumerator MoveToPlayer()
    {
        float startSpeed = 0;

        while (true)
        {
            yield return GameController.Frame;
			transform.position = Vector3.MoveTowards(transform.position, PlayerController.Instance.transform.position, startSpeed * Time.deltaTime);
            startSpeed += GameController.Instance.CoinSpeed * GameController.Instance.Speed.z;
        }        
    }
}
