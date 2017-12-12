using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour, IPickable {
    public void PickUp()
    {
        GameController.Instance.ApplyRocket();
    }
}
