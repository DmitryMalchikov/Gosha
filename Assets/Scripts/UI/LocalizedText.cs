using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string Key;
    Text text;

    private void OnEnable()
    {
        if (!text)
        {
            text = GetComponent<Text>();
        }

        StartCoroutine(WaitData());
    }

    public void SetText()
    {
        text.text = LocalizationManager.GetLocalizedValue(Key);
        if (text.text.Contains("%"))
        {
            text.text = text.text.Replace("%", Environment.NewLine);
        }
    }

    IEnumerator WaitData()
    {
        while (!LocalizationManager.IsReady)
        {
            yield return false;
        }

        SetText();
    }
}
