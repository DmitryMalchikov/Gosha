using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ContinuePanel : MonoBehaviour {

    public static ContinuePanel Instance { get; private set; }

    public Button ContinueButton;
    public Text CountDown;
	public Text ContinueCostText;
    public byte time;
    public short ContinueCost = 500;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenContinuePanel()
    {
        ContinueButton.interactable = LoginManager.User.IceCream >= ContinueCost;

		ContinueCostText.text = ContinueCost.ToString ();
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
		if (LoginManager.User.IceCream >= ContinueCost) {
			gameObject.SetActive(false);
			GameController.Instance.ContinueGameForMoney(); 
		}
    }

    public void FinishRun()
    {
        gameObject.SetActive(false);
        GameController.Instance.SuperFinish();        
    }
}
