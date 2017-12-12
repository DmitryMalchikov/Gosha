using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{

    public Text Coins;
    public Text Score;

    public float Cooldown;

    public BonusTime Magnet;
    public BonusTime Shield;
    public BonusTime Decelerator;
    public BonusTime Rocket;

    public BonusCooldown MagnetCD;
    public BonusCooldown ShieldCD;
    public BonusCooldown DeceleratorCD;
    public BonusCooldown RocketCD;

    public void Start()
    {
        MagnetCD.SetTimer(Cooldown);
        ShieldCD.SetTimer(Cooldown);
        DeceleratorCD.SetTimer(Cooldown);
        RocketCD.SetTimer(Cooldown);
    }

    public void TurdOffBonuses()
    {
        Magnet.gameObject.SetActive(false);
        Shield.gameObject.SetActive(false);
        Decelerator.gameObject.SetActive(false);
        Rocket.gameObject.SetActive(false);

        MagnetCD.gameObject.SetActive(false);
        ShieldCD.gameObject.SetActive(false);
        DeceleratorCD.gameObject.SetActive(false);
        RocketCD.gameObject.SetActive(false);
    }
}
