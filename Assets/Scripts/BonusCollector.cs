﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCollector : MonoBehaviour {

    public static BonusCollector Instance { get; private set; }

    IPickable _pickable;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "IceCream")
        {
			if (GameController.Instance.Magnet) {
				GameController.Instance.AddCoin ();
				other.gameObject.SetActive (false);
			}
        }
        else if (other.tag == "Bonus")
        {
            _pickable = other.GetComponent<IPickable>();

            if (_pickable != null)
            {
                _pickable.PickUp();
				TasksManager.Instance.CheckTasks(TasksTypes.CollectBonus);
            }
        }
    }
}
