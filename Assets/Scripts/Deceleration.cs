using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deceleration : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        GameController.Instance.ApplyDeceleration();
        gameObject.SetActive(false);
    }
}

