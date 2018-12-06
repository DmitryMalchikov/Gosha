using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Rocket : MonoBehaviour, IPickable
    {
        public void PickUp()
        {
            PlayerRocket.Instance.ApplyRocket();
            gameObject.SetActive(false);
            AudioManager.PlayEffectPickup();
        }
    }
}
