using System.Collections;
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

        if (!GameController.Paused)
        {
            PlayerController.Instance.RemoveObstcles();
            PlayerController.ResetPositionForContinue();
        }
        GameController.Paused = false;
        gameObject.SetActive(false);
        Time.timeScale = 1;        
    }
}
