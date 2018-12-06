using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Deceleration : MonoBehaviour, IPickable
    {
        public void PickUp()
        {
            SpeedController.Instance.ApplyDeceleration();
            gameObject.SetActive(false);
            AudioManager.PlayEffectPickup();
        }
    }
}

