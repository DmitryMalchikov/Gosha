using System.Collections;
using UnityEngine;

public class PlayerShield : Singleton<PlayerShield>
{
    private static bool _shieldIsOn;
    private static float _shieldTimeLeft = 0;

    public static bool ShieldIsOn
    {
        get
        {
            return _shieldIsOn;
        }
    }

    public static void OnHit()
    {
        _shieldIsOn = false;
        PlayerRigidbody.OnShieldHit();
        AudioManager.PlayShieldHitEffect();
    }    

    public static void ResetShield()
    {
        _shieldIsOn = false;
    }

    public static void TurnShieldOn()
    {
        _shieldIsOn = true;
        EffectsManager.PlayShieldEffect(true);
        Canvaser.Instance.GamePanel.Shield.Activate(true);
    }

    public static void TurnShieldOff()
    {
        _shieldIsOn = false;
        EffectsManager.PlayShieldEffect(false);
        Canvaser.Instance.GamePanel.Shield.Activate(false);
    }

    IEnumerator RemoveShield()
    {
        TurnShieldOn();

        while (_shieldTimeLeft > 0 && _shieldIsOn)
        {
            yield return CoroutineManager.Frame;
            _shieldTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Shield.SetTimer(_shieldTimeLeft);
        }

        Canvaser.Instance.GamePanel.ShieldCD.OpenCooldownPanel();
        TurnShieldOff();

        if (_shieldTimeLeft <= 0)
        {
            AudioManager.PlayEffectEnd();
        }
    }

    public void ApplyShield()
    {
        _shieldTimeLeft = GameController.Instance.ShieldTime;

        if (!_shieldIsOn)
        {
            StartCoroutine(RemoveShield());
        }
    }
}
