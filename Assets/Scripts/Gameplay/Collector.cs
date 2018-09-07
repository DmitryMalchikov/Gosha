using System.Collections;
using UnityEngine;

public class Collector : Singleton<Collector>
{ 
	Coin _coin;
	private string IceCreamTag = "IceCream";

    public BoxCollider Collider;
    public Vector3 MagnetSize = new Vector3(15, 15, 8);
    public Vector3 StandardSize = new Vector3(1, 2, 1);

    private void Start()
    {
        Collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
		if (other.transform.tag == IceCreamTag)
        {
			if (GameController.Instance.Magnet) {
				_coin = other.GetComponent<Coin> ();

				if (_coin != null) {
					_coin.OnMagnet ();
				}
			}
        }
    }

    public static void SwitchMagnetOn()
    {
        Instance.Collider.size = Instance.MagnetSize;
        GameController.Instance.Magnet = true;
        PlayerController.TurnOnEffect(EffectType.Magnet);
        AudioManager.PlayMagnetEffect();
        Canvaser.Instance.GamePanel.Magnet.Activate(true);
    }

    public static void SwitchMagnetOff()
    {
        Instance.Collider.size = Instance.StandardSize;
        GameController.Instance.Magnet = false;
        PlayerController.TurnOffEffect(EffectType.Magnet);
        AudioManager.StopEffectsSound();
        Canvaser.Instance.GamePanel.Magnet.Activate(false);
    }

    public void UseMagnet()
    {
        GameController.Instance.MagnetTimeLeft = GameController.Instance.MagnetTime;

        if (!GameController.Instance.Magnet)
        {
            StartCoroutine(DefaultSize());
        }
    }

    IEnumerator DefaultSize()
    {
        SwitchMagnetOn();

        while (GameController.Instance.MagnetTimeLeft > 0)
        {
            yield return CoroutineManager.Frame;
            GameController.Instance.MagnetTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Magnet.SetTimer(GameController.Instance.MagnetTimeLeft);
        }      

        SwitchMagnetOff();

        Canvaser.Instance.GamePanel.MagnetCD.OpenCooldownPanel();
        AudioManager.PlayEffectEnd();
    }

    public void ResetMagnet()
    {
        StopAllCoroutines();
        SwitchMagnetOff();
    }
}
