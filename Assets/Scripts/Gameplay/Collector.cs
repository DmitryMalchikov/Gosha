using System.Collections;
using UnityEngine;

public class Collector : Singleton<Collector>
{
    Coin _coin;
    private string IceCreamTag = "IceCream";
    private static float _magnetTimeLeft;
    private static bool _magnetInProgress = false;

    public BoxCollider Collider;
    public Vector3 MagnetSize = new Vector3(15, 15, 8);
    public Vector3 StandardSize = new Vector3(1, 2, 1);

    public bool MagnetInProgress
    {
        get
        {
            return _magnetInProgress;
        }
    }

    private void Start()
    {
        Collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == IceCreamTag)
        {
            if (MagnetInProgress)
            {
                _coin = other.GetComponent<Coin>();

                if (_coin != null)
                {
                    _coin.OnMagnet();
                }
            }
        }
    }

    public static void SwitchMagnetOn()
    {
        Instance.Collider.size = Instance.MagnetSize;
        _magnetInProgress = true;
        EffectsManager.PlayMagnetEffect(true);
        AudioManager.PlayMagnetEffect();
        Canvaser.Instance.GamePanel.Magnet.Activate(true);
    }

    public static void SwitchMagnetOff()
    {
        Instance.Collider.size = Instance.StandardSize;
        _magnetInProgress = false;
        EffectsManager.PlayMagnetEffect(false);
        AudioManager.StopEffectsSound();
        Canvaser.Instance.GamePanel.Magnet.Activate(false);
    }

    public void UseMagnet()
    {
        _magnetTimeLeft = GameController.Instance.MagnetTime;

        if (!MagnetInProgress)
        {
            StartCoroutine(DefaultSize());
        }
    }

    IEnumerator DefaultSize()
    {
        SwitchMagnetOn();

        while (_magnetTimeLeft > 0)
        {
            yield return CoroutineManager.Frame;
            _magnetTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Magnet.SetTimer(_magnetTimeLeft);
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
