using System;
using System.Collections;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LocalizedText : MonoBehaviour
    {
        public string Key;
        private Text _text;

        private void OnEnable()
        {
            if (!_text)
            {
                _text = GetComponent<Text>();
            }

            StartCoroutine(WaitData());
        }

        public void SetText()
        {
            _text.text = LocalizationManager.GetLocalizedValue(Key);
            if (_text.text.Contains("%"))
            {
                _text.text = _text.text.Replace("%", Environment.NewLine);
            }
        }

        private IEnumerator WaitData()
        {
            while (!LocalizationManager.IsReady)
            {
                yield return null;
            }

            SetText();
        }
    }
}
