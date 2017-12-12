using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour {

    public Text message;
    public int offset = 5;

    public void ShowMessage(string messagetoShow)
    {
        message.text = messagetoShow;
        gameObject.SetActive(true);
        StartCoroutine(MessageCloser());
    }

    IEnumerator MessageCloser()
    {
        yield return new WaitForSeconds(offset);
        gameObject.SetActive(false);
    }
}
