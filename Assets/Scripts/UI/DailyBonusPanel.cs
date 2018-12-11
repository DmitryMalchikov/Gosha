using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DailyBonusPanel : MonoBehaviour
    {
        public GameObject GetBonusButton;
        public GameObject BackButton;
        public Slider CurrentDay;

        public void Show(bool show)
        {
            SetHighlights();
            gameObject.SetActive(show);
        }

        public void SetHighlights()
        {
            CurrentDay.value = LoginManager.User.DaysInRow;
            GetBonusButton.SetActive(!LoginManager.User.GotDailyBonus);
            BackButton.SetActive(LoginManager.User.GotDailyBonus);
        }
    }
}
