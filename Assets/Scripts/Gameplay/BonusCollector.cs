using UnityEngine;

public class BonusCollector : Singleton<BonusCollector>
{
    IPickable _pickable;    

    private void OnTriggerEnter(Collider other)
    {
         _pickable = other.GetComponent<IPickable>();

         if (_pickable != null)
         {
             _pickable.PickUp();

			if (other.tag == "Bonus") {
				TasksManager.Instance.CheckTasks (TasksTypes.CollectBonus);
			}
         }
     }
}
