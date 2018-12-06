using System.Collections;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ContinuePanel : MonoBehaviour {

        public static ContinuePanel Instance { get; private set; }

        public Button ContinueButton;
        public Text CountDown;
        public Text ContinueCostText;
        public byte Time;
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
            CountDown.text = Time.ToString();
            int timer = Time;
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
}
