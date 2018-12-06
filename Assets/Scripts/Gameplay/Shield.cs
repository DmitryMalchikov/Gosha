using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Shield : MonoBehaviour, IPickable
    {
        public void PickUp()
        {
            PlayerShield.Instance.ApplyShield();
            gameObject.SetActive(false);
            AudioManager.PlayEffectPickup();
        }
    }
}
