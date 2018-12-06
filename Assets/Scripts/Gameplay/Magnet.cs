using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Magnet : MonoBehaviour, IPickable
    {
        public void PickUp()
        {
            Collector.Instance.UseMagnet();
            gameObject.SetActive(false);
            AudioManager.PlayEffectPickup();
        }
    }
}
