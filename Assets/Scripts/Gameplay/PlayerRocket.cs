using System.Collections;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class PlayerRocket : Singleton<PlayerRocket>
    {
        private static float _rocketTimeLeft = 0;

        [SerializeField]
        private float _rocketPower = 600;
        [SerializeField]
        private float _rocketHeight = 5;

        static PlayerRocket()
        {
            RocketInProgress = false;
        }

        public static bool RocketInProgress { get; private set; }

        public static bool BlockMoving{ get; private set; }

        public static void TurnRocketOff()
        {
            EffectsManager.PlayRocketEffect(false);
            RocketInProgress = false;
            BlockMoving = false;
            PlayerCollider.ColliderEnabled = true;
            Canvaser.Instance.GamePanel.Rocket.Activate(false);
            PlayerAnimator.SetRocket(false);
            PlayerRigidbody.SetInAir();
            AudioManager.StopEffectsSound();
        }

        public static void TurnRocketOn()
        {
            EffectsManager.PlayRocketEffect(true);
            RocketInProgress = true;
            BlockMoving = true;
            PlayerCollider.ColliderEnabled = false;
            PlayerCollisions.ClearCollisions();
            PlayerController.Instance.OnGround = false;
            Canvaser.Instance.GamePanel.Rocket.Activate(true);
            PlayerAnimator.SetRocket(true);
            PlayerRigidbody.TurnOnRocket(Instance._rocketPower);
        }

        public void ApplyRocket()
        {
            _rocketTimeLeft = GameController.Instance.RocketTime;

            if (!RocketInProgress)
            {
                StartCoroutine(UseRocket());
            }
        }

        IEnumerator UseRocket()
        {
            StartCoroutine(CoinGenerator.Instance.StartGeneration());

            TurnRocketOn();

            yield return new WaitUntil(() =>
            {
                _rocketTimeLeft -= Time.deltaTime;
                Canvaser.Instance.GamePanel.Rocket.SetTimer(_rocketTimeLeft);
                return PlayerController.Instance.transform.position.y >= _rocketHeight || !RocketInProgress;
            });

            if (!RocketInProgress)
            {
                yield break;
            }

            AudioManager.PlayRocketEffect();
            PlayerController.Instance.transform.position = new Vector3(PlayerController.Instance.transform.position.x, _rocketHeight, PlayerController.Instance.transform.position.z);
            PlayerCollider.ColliderEnabled = true;
            PlayerRigidbody.OnRocket();
            BlockMoving = false;

            while (_rocketTimeLeft > 0 && RocketInProgress)
            {
                _rocketTimeLeft -= Time.deltaTime;
                Canvaser.Instance.GamePanel.Rocket.SetTimer(_rocketTimeLeft);
                yield return CoroutineManager.Frame;
            }

            if (!RocketInProgress)
            {
                yield break;
            }
            TurnRocketOff();
            AudioManager.PlayEffectEnd();

            Canvaser.Instance.GamePanel.RocketCD.OpenCooldownPanel();
        }

        public static void ResetRocket()
        {
            RocketInProgress = false;
        }
    }
}
