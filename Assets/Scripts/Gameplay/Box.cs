using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Box : MonoBehaviour, IPickable
    {
        public void PickUp()
        {
            AudioManager.PlayIceCreamPickup();
            GameController.Instance.AddBox();
            transform.parent.gameObject.SetActive(false);
        }
    }
}
