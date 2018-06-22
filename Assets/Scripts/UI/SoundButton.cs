using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    //private Button _button;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AudioManager.Instance);

        if (GetComponent<Button>())
        {
            GetComponent<Button>().onClick.AddListener(() => AudioManager.PlayBtnTapSound());
        }
        else if(GetComponent<Toggle>())
        {
            GetComponent<Toggle>().onValueChanged.AddListener((value) => AudioManager.PlayBtnTapSound());
        }
    }
}
