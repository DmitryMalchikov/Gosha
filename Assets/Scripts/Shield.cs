using UnityEngine;

public class Shield : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        PlayerController.Instance.ApplyShield();
        gameObject.SetActive(false);
        AudioManager.PlayEffectPickup();
    }
}
