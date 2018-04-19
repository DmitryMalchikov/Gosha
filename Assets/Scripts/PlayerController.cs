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
    public float GravityOnPercent = .75f;
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
    public Vector3 crouch;
    public Vector3 zeroX = Vector3.zero;
    public Vector3 velBeforeCrouch;
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
        Collisions.RemoveAll(col => col.Object.layer != GroundLayer);
        transform.position = StartPos;
        animator.transform.rotation = Quaternion.identity;
        animator.SetTrigger("Reset");
        animator.SetBool(RocketHash, false);
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

            if (Mathf.Abs(transform.position.x - CurrentX) < 0.01f || back)
            {
                moveDir = Vector3.zero;
                rb.constraints = FreezeExceptJump;
                isMoving = false;
                FixPos(CurrentX);
                StartCoroutine(SetOnGround());
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

            //rb.velocity += new Vector3 (-rb.velocity.x, 0, 0);
            if (OnGround)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            CurrentX += dir * Step;
            moveDir = Vector3.right * dir;
            //            if (OnGround)
            //            {
            //                moveDir.y = -100;
            //            }
            //rb.AddForce(moveDir, ForceMode.Acceleration);
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
            //moveDir.x = rb.velocity.x;
            moveDir.y = JumpSpeed;
            rb.velocity = moveDir;
            isJumping = true;
            tempOnGround = false;

            AchievementsManager.Instance.CheckAchievements(TasksTypes.Jump);
            TasksManager.Instance.CheckTasks(TasksTypes.Jump);
        }
    }

    public void Crouch()
    {

        if (GameController.Instance.Rocket || !GameController.Instance.Started)
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
        //Collisions.Add(new CollisionInfo(){Ground = false, Object = collision.gameObject});
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
        //moveDir.x = dir * moveSpeed/2;
        //		if (OnGround) {
        //			moveDir.y = -100;
        //		}

        moveDir = Vector3.right * dir;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //rb.velocity += new Vector3 (-rb.velocity.x, -rb.velocity.y, 0);
        //rb.AddForce (moveDir, ForceMode.Acceleration);
        isMoving = true;

        hitsCount++;
        CameraFollow.Instance.ShakeCamera();
    }

    private void OnHit(Collision collision)
    {
        //Collisions.Add(new CollisionInfo(){Ground = false, Object = collision.gameObject});
        animator.transform.rotation = Quaternion.Euler(0, AnimatorRoot.rotation.eulerAngles.y - 90, 0);
        lastHit = collision.collider;
        lastHit.enabled = false;
        animator.SetTrigger(DeathHash);
        GameController.Instance.Started = false;
        animator.SetBool(StartedHash, false);
        StartCoroutine(WaitDeath());
        rb.useGravity = true;
        rb.velocity += Vector3.left * rb.velocity.x + Vector3.down * rb.velocity.y;
        animator.ResetTrigger(HitLeft);
        animator.ResetTrigger(HitRight);
        animator.ResetTrigger(Left);
        animator.ResetTrigger(Right);
        animator.ResetTrigger(CrouchHash);
        CameraFollow.Instance.offset.z = -2.5f;
        //LastGroundY = StartPos.y;
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

        Vector3 normal = collision.contacts[0].normal;
        //float angleUp = Vector3.Angle (normal, Vector3.up);
        float angleForward = Vector3.Angle(normal, Vector3.back);
        Vector2 normal2 = new Vector2(normal.x, normal.z);
        //float angle = Vector2.Angle(normal2, Vector2.down);

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
            else
            {
                OnSideHit(normal, collision);
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
        //LastTile.DisableCollider (collision.collider);
        GameController.Instance.Shield = false;
        //GameController.Instance.ShieldTimeLeft = 0;
        RemoveObstcles();
        //LastTile.ClearObstacles();
        //LastTile = null;
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

        var toRemove = Collisions.Find(col => col.Object == collision.gameObject);
        if (toRemove != null)
        {
            tempOnGround = false;
            Collisions.Remove(toRemove);
            //Collisions -= 1;
            //			if (Collisions <= 0) {
            //				Collisions = 0;
            //			}
            StartCoroutine(SetOnGround());
        }
        //}
    }

    IEnumerator SetOnGround()
    {
        int waitCount = -1;

        yield return new WaitUntil(() => { waitCount++; return !isMoving; });
        yield return new WaitForFixedUpdate();

        if (!tempOnGround)
        {
            if (Collisions.Count == 0)
            {
                if (waitCount > 0)
                {
                    transform.position += Vector3.down * .1f;
                }
                //Collisions = 0;
                OnGround = false;
                tempOnGround = true;

                if (!GameController.Instance.Rocket)
                {
                    rb.useGravity = true;
                }
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

    IEnumerator RemoveShield()
    {
        GameController.Instance.Shield = true;
        TurnOnEffect(EffectType.Shield);
        Canvaser.Instance.GamePanel.Shield.Activate(true);
        while (GameController.Instance.ShieldTimeLeft > 0 && GameController.Instance.Shield)
        {
            yield return GameController.Frame;
            GameController.Instance.ShieldTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Shield.SetTimer(GameController.Instance.ShieldTimeLeft);
        }
        Canvaser.Instance.GamePanel.Shield.Activate(false);
        Canvaser.Instance.GamePanel.ShieldCD.OpenCooldownPanel();
        GameController.Instance.Shield = false;
        TurnOffEffect(EffectType.Shield);
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

    IEnumerator WaitEffect()
    {
        yield return null;
        var obstacles = Physics.OverlapBox(transform.position + Vector3.forward * 7f, new Vector3(4f, 3f, 7f), Quaternion.identity, LayerMask.GetMask("Default", "Pickable"));
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


