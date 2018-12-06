using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Notification : MonoBehaviour
    {
        public Text count;

        public void SetCount(int number)
        {
            if (number > 0)
            {
                count.text = number.ToString();
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
