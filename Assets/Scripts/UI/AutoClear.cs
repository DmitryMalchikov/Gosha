using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class AutoClear : MonoBehaviour
    {
        private InputField _input;

        private void OnEnable()
        {
            if (!_input)
            {
                _input = GetComponent<InputField>();
            }

            _input.text = string.Empty;
        }
    }
}
