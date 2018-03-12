using System.Collections;
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
         _pickable = other.GetComponent<IPickable>();

         if (_pickable != null)
         {
             _pickable.PickUp();

			if (other.tag == "Bonus") {
				TasksManager.Instance.CheckTasks (TasksTypes.CollectBonus);
			}
         }
     }
}
