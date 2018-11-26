using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    private Button _button;

    private void OnEnable()
    {
        if (!_button)
        {
            _button = GetComponent<Button>();
        }

        Canvaser.AddButton(this);
    }

    private void OnDisable()
    {
        Canvaser.RemoveButton();
    }

    public void PressButton()
    {
        _button.onClick.Invoke();
    }
}
