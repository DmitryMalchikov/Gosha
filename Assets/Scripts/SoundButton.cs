using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    //private Button _button;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AudioManager.Instance);
        
        GetComponent<Button>().onClick.AddListener(() => AudioManager.PlayBtnTapSound());
    }
}
