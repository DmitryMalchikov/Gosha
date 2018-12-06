using Assets.Scripts.DTO;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class BonusPanel : MonoBehaviour
    {
        public Text Info;
        public string Name;
        public Toggle Active;
        public IBonus Bonus;

        public void SetInfo(Bonus bonus)
        {
            Bonus = UserBonusFactory.CreateUserBonus(bonus);
            Info.text = LocalizationManager.GetLocalizedValue(Name.ToLower()) + " (" + Bonus.Count + ")";
        }

        public void SetBonus()
        {
            if (Active.isOn)
            {
                GameController.Instance.CurrentBonus = Bonus;
            }
            else if (!Canvaser.Instance.BonusesToggleGroup.AnyTogglesOn())
            {
                GameController.Instance.CurrentBonus = null;
            }
        }
    }
}
