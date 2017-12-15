using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        GameController.Instance.AddBox();
        gameObject.SetActive(false);
    }
}
