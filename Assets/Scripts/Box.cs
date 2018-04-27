using UnityEngine;

public class Box : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        AudioManager.PlayIceCreamPickup();
        GameController.Instance.AddBox();
        transform.parent.gameObject.SetActive(false);
    }
}
