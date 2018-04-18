using UnityEngine;

public class Box : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        AudioManager.PlayIceCreamPickup();
        GameController.Instance.AddBox();
        gameObject.SetActive(false);
    }
}
