using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        private AudioSource _source;

        public AudioClip IceCreamPick;
        public AudioClip EffectPick;
        public AudioClip EffectEnd;
        public AudioClip BtnTap;
        public AudioClip FreezeStartEffect;
        public AudioClip ShieldHitEffect;
        public AudioClip CaseOpen;
        public AudioClip CardGet;
        public AudioClip Jump;
        public AudioClip SideMove;
        public AudioClip SideHit;
        public AudioClip Hit;

        [Header("Long Effects")]
        public AudioClip MagnetEffect;
        public AudioClip RocketEffect;

        private static AudioSource Source { get { return Instance._source; } }

        private void Start()
        {
            _source = GetComponent<AudioSource>();
        }

        public static void PlayIceCreamPickup()
        {
            Source.PlayOneShot(Instance.IceCreamPick, .1f);
        }

        public static void PlayBtnTapSound()
        {
            Source.PlayOneShot(Instance.BtnTap, .7f);
        }    

        public static void StopEffectsSound()
        {
            Source.Stop();
        }

        public static void PlayMagnetEffect()
        {
            Source.clip = Instance.MagnetEffect;
            Source.Play();
        }

        public static void PlayRocketEffect()
        {
            Source.clip = Instance.RocketEffect;
            Source.Play();
        }

        public static void PlayShieldHitEffect()
        {
            Source.PlayOneShot(Instance.ShieldHitEffect);
        }

        public static void PlayFreezeStartEffect()
        {
            Source.PlayOneShot(Instance.FreezeStartEffect);
        }

        public static void PlayEffectPickup()
        {
            Source.PlayOneShot(Instance.EffectPick);
        }

        public static void PlayEffectEnd()
        {
            Source.PlayOneShot(Instance.EffectEnd);
        }

        public static void PlayCaseOpen()
        {
            Source.PlayOneShot(Instance.CaseOpen);
        }

        public static void PlayCardGet()
        {
            Source.PlayOneShot(Instance.CardGet);
        }

        public static void PlayJump()
        {
            Source.PlayOneShot(Instance.Jump, .7f);
        }

        public static void PlaySideMove()
        {
            Source.PlayOneShot(Instance.SideMove, .7f);
        }

        public static void PlaySideHit()
        {
            Source.PlayOneShot(Instance.SideHit);
        }

        public static void PlayHit()
        {
            Source.PlayOneShot(Instance.Hit);
        }
    }
}
