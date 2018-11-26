using UnityEngine;

public class PlayerCollider : Singleton<PlayerCollider> {

    [SerializeField]
    private CapsuleCollider _collider;
    [SerializeField]
    private Vector3 _colliderStandCenter = new Vector3();
    [SerializeField]
    private Vector3 _colliderCrouchCenter = new Vector3(0, -0.5f, 0);
    [SerializeField]
    private float _colliderCrouchHeight = .35f;
    [SerializeField]
    private float _colliderStandHeight = .7f;

    private static bool _isCrouch = false;

    public static bool IsCrouch
    {
        get
        {
            return _isCrouch;
        }
    }

    public static bool ColliderEnabled
    {
        get
        {
            return Instance._collider.enabled;
        }
        set
        {
            Instance._collider.enabled = value;
        }
    }

    private static CapsuleCollider InstanceCollider
    {
        get
        {
            return Instance._collider;
        }
    }

    public static void Crouch()
    {
        InstanceCollider.height = Instance._colliderCrouchHeight;
        InstanceCollider.center = Instance._colliderCrouchCenter;
        _isCrouch = true;
    }

    public static void StandUp()
    {
        InstanceCollider.height = Instance._colliderStandHeight;
        InstanceCollider.center = Instance._colliderStandCenter;
        _isCrouch = false;
    }

    public static void ResetCollider()
    {
        StandUp();
        InstanceCollider.enabled = true;
    }
}
