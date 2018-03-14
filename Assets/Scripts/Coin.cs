using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IPickable
{
	private bool _reset = false;
	private Vector3 _defaultPosition;

    public virtual void PickUp()
    {   
      	GameController.Instance.AddCoin();
      	gameObject.SetActive(false);         
    }

	void Start(){
		_defaultPosition = transform.position;
	}

	void OnEnable(){
		if (_reset) {
			_reset = false;
			transform.position = _defaultPosition;
		}
	}

	public void OnMagnet(){
		_reset = true;
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
