using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsPanel : MonoBehaviour {

    public Image img;
    public Text txt;

    public void OpenAds()
    {
        gameObject.SetActive(true);
    }
}
