using UnityEngine;

public class Shield : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        PlayerShield.Instance.ApplyShield();
        gameObject.SetActive(false);
        AudioManager.PlayEffectPickup();
    }
}
