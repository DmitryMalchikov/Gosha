using System.Collections;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class PlayerShield : Singleton<PlayerShield>
    {
        private static float _shieldTimeLeft = 0;

        public static bool ShieldIsOn { get; private set; }

        public static void OnHit()
        {
            ShieldIsOn = false;
            PlayerRigidbody.OnShieldHit();
            AudioManager.PlayShieldHitEffect();
        }    

        public static void ResetShield()
        {
            ShieldIsOn = false;
        }

        public static void TurnShieldOn()
        {
            ShieldIsOn = true;
            EffectsManager.PlayShieldEffect(true);
            Canvaser.Instance.GamePanel.Shield.Activate(true);
        }

        public static void TurnShieldOff()
        {
            ShieldIsOn = false;
            EffectsManager.PlayShieldEffect(false);
            Canvaser.Instance.GamePanel.Shield.Activate(false);
        }

        private IEnumerator RemoveShield()
        {
            TurnShieldOn();

            while (_shieldTimeLeft > 0 && ShieldIsOn)
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

            if (!ShieldIsOn)
            {
                StartCoroutine(RemoveShield());
            }
        }
    }
}
