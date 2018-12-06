using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class EffectsManager : Singleton<EffectsManager>
    {
        [SerializeField]
        private Effect _iceEffect;
        [SerializeField]
        private Effect _magnetEffect;
        [SerializeField]
        private Effect _shieldEffect;
        [SerializeField]
        private Effect _rocketEffect;
        [SerializeField]
        private ParticleSystem _iceCreamPicked;
        [SerializeField]
        private ParticleSystem _obstaclesEffect;

        public static void PlayIceEffect(bool play)
        {
            Instance._iceEffect.Show(play, false);
        }

        public static void PlayMagnetEffect(bool play)
        {
            Instance._magnetEffect.Show(play, false);
        }

        public static void PlayShieldEffect(bool play)
        {
            Instance._shieldEffect.Show(play, true);
        }
        public static void PlayRocketEffect(bool play)
        {
            Instance._rocketEffect.Show(play, true);
        }

        public static void PlayIceCreamPicked()
        {
            Instance._iceCreamPicked.Emit(1);
        }

        public static void PlayObstacleEffect()
        {
            Instance._obstaclesEffect.Play();
        }

        public static void TurnOffEffects()
        {
            PlayIceEffect(false);
            PlayMagnetEffect(false);
            PlayShieldEffect(false);
            PlayRocketEffect(false);
        }
    }
}
