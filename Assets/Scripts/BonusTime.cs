using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusTime : MonoBehaviour {

    public Text Timer;

	public void SetTimer(float timeLeft)
    {
        Timer.text = timeLeft.ToString("0.0");
    }
}
