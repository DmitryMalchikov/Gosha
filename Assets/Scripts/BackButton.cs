using System.Collections;
using System.Collections.Generic;
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

    public void PressButton()
    {
        _button.onClick.Invoke();
    }
    //private void OnDisable()
    //{
    //    Canvaser.RemoveButton(this);
    //}
}
