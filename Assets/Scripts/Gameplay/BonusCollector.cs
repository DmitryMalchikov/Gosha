using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class BonusCollector : Singleton<BonusCollector>
    {
        private IPickable _pickable;

        private void OnTriggerEnter(Collider other)
        {
            _pickable = other.GetComponent<IPickable>();

            if (_pickable == null) return;
            _pickable.PickUp();

            if (other.tag == "Bonus")
            {
                TasksManager.Instance.CheckTasks(TasksTypes.CollectBonus);
            }
        }
    }
}
