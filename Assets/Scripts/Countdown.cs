using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{

    public Text SecondsLeft;

    private WaitForSeconds second = new WaitForSeconds(1);

    private void OnEnable()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        var time = 3;

        while (time > 0)
        {
            SecondsLeft.text = time.ToString();
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1));
            time--;
        }

        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
