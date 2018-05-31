using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    #region Constants
    public const RigidbodyConstraints FreezeExceptJump = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
    #endregion

    #region Hashes
    static int CrouchHash = Animator.StringToHash("Crouch");
    static int JumpHash = Animator.StringToHash("Jump");
    static int DeathHash = Animator.StringToHash("Death");
    static int HitLeft = Animator.StringToHash("HitLeft");
    static int HitRight = Animator.StringToHash("HitRight");
    static int Left = Animator.StringToHash("Left");
    static int Right = Animator.StringToHash("Right");
    static int OnGroundHash = Animator.StringToHash("OnGround");
    public static int StartedHash = Animator.StringToHash("Started");
    public static int RocketHash = Animator.StringToHash("Rocket");
    #endregion

    public static PlayerController Instance;

    public bool UseHardTouch = true;
    public float Step = 2;
    [Range(0, 1)]
    public float GravityOnPercent = .75f;
    public float LowDelta = 1.5f;
    public float CurrentX = 0;
    public float nextStep;
    public float dir;
    public float moveSpeed = 1;
    public float JumpSpeed = 1;
    public float CrouchPower = 1;
    public bool isJumping;
    public bool isCrouching = false;
    public bool isMoving;
    public List<CollisionInfo> Collisions = new List<CollisionInfo>();

    public Transform AnimatorRoot;
    public Animator animator;
    public CapsuleCollider col;
    public float MaxCollisionAngle;
    public float MinCollisionAngle;
    public int MaxHitsCount = 2;
    public float DeltaXOffset = .1f;

    [Header("Effects")]
    public Effect IceEffect;
    public Effect MagnetEffect;
    public Effect ShieldEffect;
    public Effect RocketEffect;
    public ParticleSystem IceCreamPicked;
    public ParticleSystem ObstaclesEffect;


    private Vector3 moveDir;
    private float FallDistance;

    [Space(20)]
    public Rigidbody rb;

    public bool OnRamp = false;
    public Vector3 StartPos = new Vector3(0f, 0.5f, -4f);

    public Vector3 ColliderStand = new Vector3();
    public Vector3 ColliderCrouch = new Vector3(0, -0.5f, 0);

    [HideInInspector]
    public Tile LastTile;

    private float minY = 0;
    private int DefaultLayer;
    private int GroundLayer;

    public bool OnGround
    {
        get { return _onGround; }
        set
        {
            _onGround = value;
            animator.SetBool(OnGroundHash, _onGround);
        }
    }
    bool _onGround = true;

    int environmentMask;
    public float LastGroundY = 0;
    public Animator PlayerAnimator;
    public Transform Position;

    private Collider lastHit;
    private Vector3 velocityBeforePhysics;
    private byte hitsCount;
    private SuitInfo[] _suitsItems;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Instance = this;
    }

    public void ResetSas()
    {
        StopAllCoroutines();
        StandUp();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        Collisions.RemoveAll(col => col.Object.name != "Ground");
        transform.position = StartPos;
        animator.transform.rotation = Quaternion.identity;
        animator.SetTrigger("Reset");
        OnGround = true;
        CurrentX = 0;
        hitsCount = 0;
    }

    void Start()
    {
        FallDistance = Step * (1f - GravityOnPercent);
        environmentMask = LayerMask.GetMask("Ground", "Default");
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        PlayerAnimator = GetComponent<Animator>();
        _suitsItems = GetComponentsInChildren<SuitInfo>();
        DefaultLayer = LayerMask.NameToLayer("Default");
        GroundLayer = LayerMask.NameToLayer("Ground");
        PutOnSuit(PlayerPrefs.GetString("CurrentSuit"));
    }

    private void Update()
    {
        if ((OnGround && tempOnGround) || GameController.Instance.Rocket)
        {
            LastGroundY = transform.position.y;
        }
        else if (LastGroundY > transform.position.y && rb.velocity.y < 0)
        {
            if (transform.position.y >= minY)
            {
                LastGroundY = transform.position.y;
            }
        }

        if (!GameController.Instance.Started)
            return;

        if (((!OnGround || !tempOnGround) && rb.velocity.y > 0 && !isMoving) && !OnRamp)
        {
            StickToGround();
        }

        if (isMoving)
        {
            bool back = false;
            back = dir == -1 ? transform.position.x < CurrentX : transform.position.x > CurrentX;

            if (!rb.useGravity && Mathf.Abs(transform.position.x - CurrentX) < FallDistance && !GameController.Instance.Rocket)
            {
                if (Collisions.Count == 0)
                {
                    rb.useGravity = true;
                    rb.constraints = FreezeExceptJump;
                }
            }

            if (Mathf.Abs(transform.position.x - CurrentX) < 0.01f || back)
            {
                moveDir = Vector3.zero;
                rb.constraints = FreezeExceptJump;
                isMoving = false;
                FixPos(CurrentX);
                StartCoroutine(SetOnGround());
                if (Collisions.Count == 0)
                {
                    SetPositionAfterMove();
                }
            }
            else
            {
                transform.Translate(moveDir * moveSpeed * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        velocityBeforePhysics = rb.velocity;
    }

    void LateUpdate()
    {
        if (OnRamp)
            StickToGround();
    }

    public void StickToGround()
    {
        if (!isJumping && !GameController.Instance.Rocket)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), out hit, 1.5f, environmentMask))
            {
                transform.position = hit.point + Vector3.up * .4f;
                rb.velocity += Vector3.down * rb.velocity.y;
                tempOnGround = true;
                OnGround = true;
            }
        }
    }

    public void Move(bool right)
    {
        if (GameController.Instance.BlockMoving) return;

        dir = right ? 1 : -1;
        if (CurrentX != dir * Step)
        {
            if (GameController.Instance.Rocket || OnGround)
            {
                if (right)
                {
                    animator.SetTrigger(Right);
                }
                else
                {
                    animator.SetTrigger(Left);
                }
            }

            if (OnGround)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            if (!GameController.Instance.Rocket)
            {
                AudioManager.PlaySideMove();
            }

            CurrentX += dir * Step;
            moveDir = Vector3.right * dir;
            isMoving = true;
        }
    }

    public void Jump()
    {
        if (GameController.Instance.Rocket)
            return;

        StartCoroutine(StartJump());
    }

    IEnumerator StartJump()
    {
        yield return new WaitUntil(() => isMoving == false && tempOnGround == true);

        if (col.height == 0.35f)
            StandUp();

        if (OnGround)
        {
            animator.SetTrigger(JumpHash);
            Vector3 jumpDir = Vector3.up * JumpSpeed;
            rb.AddForce(jumpDir, ForceMode.VelocityChange);
            rb.useGravity = true;
            rb.constraints = FreezeExceptJump;
            isJumping = true;
            tempOnGround = false;

            AudioManager.PlayJump();
            AchievementsManager.Instance.CheckAchievements(TasksTypes.Jump);
            TasksManager.Instance.CheckTasks(TasksTypes.Jump);
        }
    }

    public void Crouch()
    {

        if (GameController.Instance.Rocket || !GameController.Instance.Started || isCrouching)
            return;

        if (!OnGround)
        {
            rb.AddForce(Vector3.down * CrouchPower, ForceMode.Acceleration);
        }

        isCrouching = true;
        col.center = ColliderCrouch;
        col.height = 0.35f;

        animator.SetTrigger(CrouchHash);

        StartCoroutine(WaitForCrouch());
    }

    private void OnSideHit(Vector3 normal, Collision collision)
    {
        dir = Mathf.Sign(normal.x);
        if (OnGround)
        {
            if (dir == 1)
            {
                animator.SetTrigger(HitLeft);
            }
            else
            {
                animator.SetTrigger(HitRight);
            }
        }
        if (CurrentX != dir * Step)
        {
            CurrentX += dir * Step;
        }

        moveDir = Vector3.right * dir;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        isMoving = true;

        hitsCount++;
        CameraFollow.Instance.ShakeCamera();
        AudioManager.PlaySideHit();
    }

    private void OnHit(Collision collision)
    {
        animator.transform.rotation = Quaternion.Euler(0, AnimatorRoot.rotation.eulerAngles.y - 90, 0);
        lastHit = collision.collider;
        lastHit.enabled = false;
        animator.SetTrigger(DeathHash);
        GameController.Instance.Started = false;
        animator.SetBool(StartedHash, false);
        StartCoroutine(WaitDeath());
        rb.useGravity = true;
        rb.velocity += Vector3.left * rb.velocity.x + Vector3.down * rb.velocity.y;
        ResetTriggers();
        CameraFollow.Instance.offset.z = -3.2f;
        Canvaser.Instance.GamePanel.TurdOffBonuses();
        GameController.TurnOffAllBonuses();
        AudioManager.PlayHit();
    }

    private void ResetTriggers()
    {
        animator.ResetTrigger(HitLeft);
        animator.ResetTrigger(HitRight);
        animator.ResetTrigger(Left);
        animator.ResetTrigger(Right);
        animator.ResetTrigger(CrouchHash);
        animator.ResetTrigger("Reset");
        animator.ResetTrigger(JumpHash);
    }

    private void OnGrounded(Collision collision)
    {
        if (minY == 0)
        {
            minY = transform.position.y;
        }

        if (!Collisions.Any(col => col.Object == collision.gameObject))
        {
            Collisions.Add(new CollisionInfo() { Ground = true, Object = collision.gameObject });
        }
        OnGround = true;
        tempOnGround = true;
        isJumping = false;
        rb.useGravity = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != DefaultLayer && collision.gameObject.layer != GroundLayer)
        {
            return;
        }

        if (collision.gameObject.CompareTag("HardObstacle"))
        {
            if (GameController.Instance.Shield)
            {
                ShieldHit();
            }
            else
            {
                OnHit(collision);
            }
            return;
        }

        Vector3 normal = collision.contacts[0].normal;
        float angleForward = Vector3.Angle(normal, Vector3.back);
        Vector2 normal2 = new Vector2(normal.x, normal.z);

        if (Mathf.Abs(normal.x) < 0.1f && normal.y > 0 && angleForward != 0)
        {
            OnGrounded(collision);
        }
        else if (normal2 != Vector2.zero && angleForward <= MaxCollisionAngle && angleForward >= MinCollisionAngle && hitsCount < MaxHitsCount)
        {
            if (GameController.Instance.Shield)
            {
                ShieldHit();
            }
            else if ((transform.position.y - collision.contacts[0].point.y) <= LowDelta)
            {
                OnSideHit(normal, collision);
            }
            else
            {
                OnGrounded(collision);
            }
            //side hit
        }
        else
        {
            if (GameController.Instance.Shield)
            {
                ShieldHit();
            }
            else
            {
                OnHit(collision);
            }
        }
    }

    IEnumerator WaitDeath()
    {
        TurnOffEffects();
        yield return new WaitForSeconds(1.8f);

        lastHit.enabled = true;
        GameController.Instance.FinishGame();
    }

    bool tempOnGround = true;

    private void ShieldHit()
    {
        GameController.Instance.Shield = false;
        RemoveObstcles();
        rb.velocity = velocityBeforePhysics;
        AudioManager.PlayShieldHitEffect();
    }

    private void MoveToPosition(Vector3 position)
    {
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != DefaultLayer && collision.gameObject.layer != GroundLayer)
        {
            return;
        }

        var toRemove = Collisions.FindAll(col => col.Object == collision.gameObject);
        if (toRemove != null && toRemove.Count > 0)
        {
            tempOnGround = false;
            for (int i = 0; i < toRemove.Count; i++)
            {
                Collisions.Remove(toRemove[i]);
            }
            StartCoroutine(SetOnGround());
        }
    }

    private void SetPositionAfterMove()
    {
        if (GameController.Instance.Rocket)
            return;

        if (!isJumping)
        {
            transform.position -= Vector3.up * 0.17f;
        }
    }

    IEnumerator SetOnGround()
    {
        yield return new WaitUntil(() => !isMoving);
        yield return new WaitForFixedUpdate();

        if (!tempOnGround)
        {
            if (Collisions.Count == 0)
            {
                OnGround = false;
                tempOnGround = true;

                if (!GameController.Instance.Rocket)
                {
                    rb.useGravity = true;
                    rb.constraints = FreezeExceptJump;
                }
            }
            else
            {
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

    }

    IEnumerator WaitForCrouch()
    {
        yield return new WaitUntil(() => OnGround == true);


        yield return new WaitForSeconds(0.8f);
        StandUp();
    }

    void StandUp()
    {
        col.height = .7f;
        col.center = ColliderStand;
        isCrouching = false;
    }

    public void FixPos(float posX)
    {
        transform.position = new Vector3(posX, transform.position.y, transform.position.z);
    }

    public void PickUp(Collision other)
    {
        other.transform.GetComponent<IPickable>().PickUp();
    }


    public void ApplyShield()
    {
        GameController.Instance.ShieldTimeLeft = GameController.Instance.ShieldTime;

        if (!GameController.Instance.Shield)
        {
            StartCoroutine(RemoveShield());
        }
    }

    public static void TurnShieldOn()
    {
        GameController.Instance.Shield = true;
        TurnOnEffect(EffectType.Shield);
        Canvaser.Instance.GamePanel.Shield.Activate(true);
    }

    public static void TurnShieldOff()
    {
        GameController.Instance.Shield = false;
        TurnOffEffect(EffectType.Shield);
        Canvaser.Instance.GamePanel.Shield.Activate(false);
    }

    IEnumerator RemoveShield()
    {
        TurnShieldOn();

        while (GameController.Instance.ShieldTimeLeft > 0 && GameController.Instance.Shield)
        {
            yield return GameController.Frame;
            GameController.Instance.ShieldTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Shield.SetTimer(GameController.Instance.ShieldTimeLeft);
        }

        Canvaser.Instance.GamePanel.ShieldCD.OpenCooldownPanel();
        TurnShieldOff();

        if (GameController.Instance.ShieldTimeLeft <= 0)
        {
            AudioManager.PlayEffectEnd();
        }
    }

    public void RemoveObstcles()
    {
        ObstaclesEffect.Play();

        StartCoroutine(WaitEffect());
    }

    public static void ResetPositionForContinue()
    {
        Instance.animator.SetBool(StartedHash, true);
        Instance.animator.SetTrigger("Reset");
        Instance.animator.transform.rotation = new Quaternion();
        Instance.transform.position += Vector3.right * (Instance.CurrentX - Instance.transform.position.x);
    }

    IEnumerator WaitEffect()
    {
        yield return null;
        var obstacles = Physics.OverlapBox(transform.position + Vector3.forward * 7f, new Vector3(4f, 3f, 8.5f), Quaternion.identity, LayerMask.GetMask("Default", "Pickable"));
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].transform.parent.gameObject.SetActive(false);
        }
    }

    public void PutOnSuit(string suitName)
    {
        for (int i = 0; i < _suitsItems.Length; i++)
        {
            _suitsItems[i].gameObject.SetActive(_suitsItems[i].SuitName == suitName);
        }
    }

    public void TakeOffSuits()
    {
        for (int i = 0; i < _suitsItems.Length; i++)
        {
            _suitsItems[i].gameObject.SetActive(false);
        }
    }

    public static void TurnOnEffect(EffectType type)
    {
        switch (type)
        {
            case EffectType.Freeze:
                Instance.IceEffect.Play();
                break;
            case EffectType.Magnet:
                Instance.MagnetEffect.Play();
                break;
            case EffectType.Rocket:
                Instance.RocketEffect.Play();
                break;
            case EffectType.Shield:
                Instance.ShieldEffect.Play();
                break;
        }
    }

    public static void TurnOffEffect(EffectType type)
    {
        switch (type)
        {
            case EffectType.Freeze:
                Instance.IceEffect.Stop();
                break;
            case EffectType.Magnet:
                Instance.MagnetEffect.Stop();
                break;
            case EffectType.Rocket:
                Instance.RocketEffect.Stop(true);
                break;
            case EffectType.Shield:
                Instance.ShieldEffect.Stop(true);
                break;
        }
    }

    public static void TurnOffEffects()
    {
        Instance.IceEffect.Stop();
        Instance.MagnetEffect.Stop();
        Instance.RocketEffect.Stop(true);
        Instance.ShieldEffect.Stop(true);
    }

    public static void PickIceCream()
    {
        Instance.IceCreamPicked.Emit(1);
    }
}

public enum EffectType
{
    Shield, Freeze, Magnet, Rocket
}

public class CollisionInfo
{
    public bool Ground { get; set; }
    public GameObject Object { get; set; }
}


