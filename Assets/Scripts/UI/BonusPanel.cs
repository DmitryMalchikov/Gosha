using UnityEngine;
using UnityEngine.UI;

public class BonusPanel : MonoBehaviour
{
    public Text Info;
    public string Name;
    public int Count;
    public Toggle Active;
    public Bonus Bonus;

    public void SetInfo(Bonus bonus)
    {
        Bonus = bonus;

        if (Bonus != null)
        {
            Count = bonus.Amount;
        }
        else
        {            
            Count = 0;
        }

        Info.text = LocalizationManager.GetLocalizedValue(Name.ToLower()) + " (" + Count + ")";
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
