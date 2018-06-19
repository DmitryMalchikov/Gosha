using UnityEngine;

public class Deceleration : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        GameController.Instance.ApplyDeceleration();
        gameObject.SetActive(false);
        AudioManager.PlayEffectPickup();
    }
}

