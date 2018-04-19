using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {

    public static Collector Instance { get; private set; }

	Coin _coin;
	private string IceCreamTag = "IceCream";

    public BoxCollider Collider;
    public Vector3 MagnetSize = new Vector3(15, 15, 8);
    public Vector3 StandardSize = new Vector3(1, 2, 1);

    private void Awake()
    {
        Instance = this;
    }

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

    public void UseMagnet()
    {
        GameController.Instance.MagnetTimeLeft = GameController.Instance.MagnetTime;

        if (!GameController.Instance.Magnet)
        {
            //Collider.center = Vector3.forward * (MagnetSize.z / 2 - 1);
            Collider.size = MagnetSize;

            StartCoroutine(DefaultSize());
        }
    }

    IEnumerator DefaultSize()
    {
        GameController.Instance.Magnet = true;
		PlayerController.TurnOnEffect (EffectType.Magnet);
        AudioManager.PlayMagnetEffect();

        Canvaser.Instance.GamePanel.Magnet.Activate(true);
        while (GameController.Instance.MagnetTimeLeft > 0)
        {
            yield return GameController.Frame;
            GameController.Instance.MagnetTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Magnet.SetTimer(GameController.Instance.MagnetTimeLeft);
        }
        Canvaser.Instance.GamePanel.Magnet.Activate(false);
        Canvaser.Instance.GamePanel.MagnetCD.OpenCooldownPanel();

        //Collider.center = Vector3.forward * (StandardSize.z / 2 - 1);
        Collider.size = StandardSize;
        GameController.Instance.Magnet = false;
		PlayerController.TurnOffEffect (EffectType.Magnet);       
        AudioManager.StopEffectsSound();
        AudioManager.PlayEffectEnd();
    }

    public void ResetSas()
    {
        StopAllCoroutines();
        Collider.size = StandardSize;
        GameController.Instance.Magnet = false;
    }
}
