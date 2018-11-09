using UnityEngine;

public class Deceleration : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        SpeedController.Instance.ApplyDeceleration();
        gameObject.SetActive(false);
        AudioManager.PlayEffectPickup();
    }
}

