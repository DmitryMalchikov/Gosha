using UnityEngine;

public class Rocket : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        PlayerRocket.Instance.ApplyRocket();
        gameObject.SetActive(false);
        AudioManager.PlayEffectPickup();
    }
}
