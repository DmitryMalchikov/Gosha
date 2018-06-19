using UnityEngine;
using UnityEngine.UI;

public class DailyBonusPanel : MonoBehaviour
{
    public GameObject GetBonusButton;
    public GameObject BackButton;
    public Slider CurrentDay;

    public void SetHighlights()
    {
        Debug.Log(LoginManager.Instance.User.DaysInRow);
        CurrentDay.value = LoginManager.Instance.User.DaysInRow;
        GetBonusButton.SetActive(!LoginManager.Instance.User.GotDailyBonus);
        BackButton.SetActive(LoginManager.Instance.User.GotDailyBonus);
    }
}
