using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCollector : MonoBehaviour {

    public static BonusCollector Instance { get; private set; }

    IPickable _pickable;

    public BoxCollider Collider;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "IceCream")
        {
            GameController.Instance.AddCoin();
            other.gameObject.SetActive(false);
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
