using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinuePanel : MonoBehaviour {

    public static ContinuePanel Instance { get; private set; }

    public Button ContinueButton;
    public Text CountDown;
    public int time;
    public int ContinueCost = 2000;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenContinuePanel()
    {
        ContinueButton.interactable = LoginManager.Instance.User.IceCream >= ContinueCost;

        gameObject.SetActive(true);
        StartCoroutine(WaitToClosePanel());
    }

    IEnumerator WaitToClosePanel ()
    {
        CountDown.text = time.ToString();
        int timer = time;
        while(timer > 0)
        {
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1));
            timer--;
            CountDown.text = timer.ToString();
        }
        FinishRun();
    }

    public void Continue()
    {
        ScoreManager.Instance.ContinueForMoney();
    }

    public void FinishRun()
    {
        GameController.Instance.SuperFinish();
        gameObject.SetActive(false);
    }
}
