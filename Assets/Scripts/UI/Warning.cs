using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class Warning : MonoBehaviour
    {
        public Text message;
        public int offset = 5;

        public void ShowMessage(string messageToShow)
        {
            message.text = messageToShow;
            gameObject.SetActive(true);
            StartCoroutine(MessageCloser());
        }

        IEnumerator MessageCloser()
        {
            yield return new WaitForSeconds(offset);
            gameObject.SetActive(false);
        }
    }
}
