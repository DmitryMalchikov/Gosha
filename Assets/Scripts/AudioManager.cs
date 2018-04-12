using UnityEngine;
using UnityEngine.EventSystems;

public class AudioManager : MonoBehaviour
{
    private AudioSource _source;

    public static AudioManager Instance;

    public AudioClip IceCreamPick;
    public AudioClip EffectPick;
    public AudioClip EffectEnd;
    public AudioClip BtnTap;
    public AudioClip FreezeStartEffect;
    public AudioClip ShieldHitEffect;
    public AudioClip CaseOpen;
    public AudioClip CardGet;

    [Header("Long Effects")]
    public AudioClip MagnetEffect;
    public AudioClip RocketEffect;

    private static AudioSource Source { get { return Instance._source; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public static void PlayIceCreamPickup()
    {
        Source.PlayOneShot(Instance.IceCreamPick, .7f);
    }

    public static void PlayBtnTapSound()
    {
       Source.PlayOneShot(Instance.BtnTap);
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
}
