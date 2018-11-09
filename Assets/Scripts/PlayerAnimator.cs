using UnityEngine;

public class PlayerAnimator : Singleton<PlayerAnimator>
{
    #region Hashes
    private static readonly int CrouchHash = Animator.StringToHash("Crouch");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private static readonly int HitLeftHash = Animator.StringToHash("HitLeft");
    private static readonly int HitRightHash = Animator.StringToHash("HitRight");
    private static readonly int LeftHash = Animator.StringToHash("Left");
    private static readonly int RightHash = Animator.StringToHash("Right");
    private static readonly int OnGroundHash = Animator.StringToHash("OnGround");
    private static readonly int StartedHash = Animator.StringToHash("Started");
    private static readonly int RocketHash = Animator.StringToHash("Rocket");
    private static readonly int RocketTriggerHash = Animator.StringToHash("RocketTrigger");
    private static readonly int ResetTriggerHash = Animator.StringToHash("Reset");
    private static readonly int GroundNearHash = Animator.StringToHash("GroundNear");
    #endregion

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _animatorRoot;
    [SerializeField]
    private float _groundNearDistance = 0.5f;

    private static Animator InstaceAnimator
    {
        get
        {
            return Instance._animator;
        }
    }

    public static void SetRocket(bool isOn)
    {
        InstaceAnimator.SetBool(RocketHash, isOn);

        if (isOn)
        {
            InstaceAnimator.SetTrigger(RocketTriggerHash);
        }
        else
        {
            InstaceAnimator.ResetTrigger(RocketTriggerHash);
        }
    }

    public static void Started(bool start)
    {
        SetStarted(start);
        SetResetTrigger(!start);
        InstaceAnimator.transform.rotation = Quaternion.identity;
    }

    public static void Continue(bool toContinue)
    {
        SetStarted(toContinue);
        SetResetTrigger(toContinue);
        InstaceAnimator.transform.rotation = Quaternion.identity;
    }

    public static void SetStarted(bool isOn)
    {
        InstaceAnimator.SetBool(StartedHash, isOn);
    }

    public static void SetOnGround(bool isOn)
    {
        InstaceAnimator.SetBool(OnGroundHash, isOn);
    }

    public static void SetResetTrigger(bool set)
    {
        if (set)
        {
            InstaceAnimator.SetTrigger(ResetTriggerHash);
        }
        else
        {
            InstaceAnimator.ResetTrigger(ResetTriggerHash);
        }
    }

    public static void SetTurnTrigger(bool right)
    {
        if (right)
        {
            InstaceAnimator.SetTrigger(RightHash);
        }
        else
        {
            InstaceAnimator.SetTrigger(LeftHash);
        }
    }

    public static void SetSideHitTrigger(float sign)
    {
        if (sign == 1)
        {
            InstaceAnimator.SetTrigger(HitLeftHash);
        }
        else
        {
            InstaceAnimator.SetTrigger(HitRightHash);
        }
    }

    public static void SetJumpTrigger()
    {
        InstaceAnimator.SetTrigger(JumpHash);
    }

    public static void SetCrouchTrigger()
    {
        InstaceAnimator.SetTrigger(CrouchHash);
    }

    public static void SetDeath()
    {
        InstaceAnimator.SetTrigger(DeathHash);
        InstaceAnimator.transform.rotation = Quaternion.Euler(0, Instance._animatorRoot.rotation.eulerAngles.y - 90, 0);
        SetStarted(false);
        ResetTriggers();
    }

    public static void SetGroundNear()
    {
        bool successRaycast = Physics.Raycast(new Ray(Instance.transform.position, Vector3.down), Instance._groundNearDistance, Masks.EnvironmentMask);
        InstaceAnimator.SetBool(GroundNearHash, successRaycast);
    }

    public static void ResetTriggers()
    {
        InstaceAnimator.ResetTrigger(HitLeftHash);
        InstaceAnimator.ResetTrigger(HitRightHash);
        InstaceAnimator.ResetTrigger(LeftHash);
        InstaceAnimator.ResetTrigger(RightHash);
        InstaceAnimator.ResetTrigger(CrouchHash);
        InstaceAnimator.ResetTrigger(ResetTriggerHash);
        InstaceAnimator.ResetTrigger(JumpHash);
    }
}
