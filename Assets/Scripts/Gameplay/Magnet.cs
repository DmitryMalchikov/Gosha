using UnityEngine;

public class Magnet : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        Collector.Instance.UseMagnet();
        gameObject.SetActive(false);
        AudioManager.PlayEffectPickup();
    }
}
