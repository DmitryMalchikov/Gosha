using UnityEngine;

public class PlayerRigidbody : Singleton<PlayerRigidbody>
{
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private Vector3 JumpSpeed;
    [SerializeField]
    private Vector3 _crouchPower = Vector3.down;

    private static Rigidbody InstanceRigidbody
    {
        get
        {
            return Instance._rigidbody;
        }
    }

    public static Vector3 Velocity
    {
        get
        {
            return InstanceRigidbody.velocity;
        }
        private set
        {
            InstanceRigidbody.velocity = value;
        }
    }

    public static bool UseGravity
    {
        get
        {
            return InstanceRigidbody.useGravity;
        }
        set
        {
            InstanceRigidbody.useGravity = value;
        }
    }

    private static Vector3 _velocityBeforePhysics;

    private void Start()
    {
        
    }

    public static void FreezeAll()
    {
        InstanceRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public static void FreezeExceptJump()
    {
        InstanceRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
    }

    public static void AddForce(Vector3 force, ForceMode mode)
    {
        InstanceRigidbody.AddForce(force, mode);
    }

    public static void ResetRigidbody()
    {
        InstanceRigidbody.useGravity = false;
        FreezeAll();
    }

    public static void SetInAir()
    {
        UseGravity = true;
        FreezeExceptJump();
    }

    public static void TurnOnRocket(float rocketPower)
    {
        Velocity += Vector3.up * (rocketPower - Velocity.y);
        UseGravity = false;
        FreezeExceptJump();
    }

    public static void OnRocket()
    {
        Velocity = Vector3.zero;
        FreezeAll();
    }

    public static void Jump()
    {
        AddForce(Instance.JumpSpeed, ForceMode.VelocityChange);
        UseGravity = true;
        FreezeExceptJump();
    }

    public static void OnHit()
    {
        UseGravity = true;
        Velocity += Vector3.left * Velocity.x + Vector3.down * Velocity.y;
        FreezeExceptJump();
    }

    public static void OnShieldHit()
    {
        Velocity = _velocityBeforePhysics;
    }

    public static void ResetVelocity()
    {
        Velocity = Vector3.zero;
    }

    public static void MoveToGround(bool onGround)
    {
        if (!onGround)
        {
            AddForce(Instance._crouchPower, ForceMode.Acceleration);
            PlayerAnimator.SetGroundNear();
        }
        else
        {
            ResetVelocity();
        }
    }

    private void FixedUpdate()
    {
        _velocityBeforePhysics = Velocity;
    }
}
