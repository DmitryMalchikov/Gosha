using UnityEngine;
using UnityEngine.UI;

public class DailyBonusPanel : MonoBehaviour
{
    public GameObject GetBonusButton;
    public GameObject BackButton;
    public Slider CurrentDay;

    public void SetHighlights()
    {
        Debug.Log(LoginManager.User.DaysInRow);
        CurrentDay.value = LoginManager.User.DaysInRow;
        GetBonusButton.SetActive(!LoginManager.User.GotDailyBonus);
        BackButton.SetActive(LoginManager.User.GotDailyBonus);
    }
}
