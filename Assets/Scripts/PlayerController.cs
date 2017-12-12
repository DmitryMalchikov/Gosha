using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Hashes
    static int CrouchHash = Animator.StringToHash("Crouch");
    static int JumpHash = Animator.StringToHash("Jump");
    static int DeathHash = Animator.StringToHash("Death");
    public static int StartedHash = Animator.StringToHash("Started");
    public static int RocketHash = Animator.StringToHash("Rocket");
    #endregion

    public static PlayerController Instance;

    public float Step = 2;
    public float CurrentX = 0;
    public float nextStep;
    public float dir;
    public float moveSpeed = 1;
    public float JumpSpeed = 1;
    public float CrouchPower = 1;
    public bool isJumping;
    public bool isCrouching = false;
    public bool isMoving;
    public int Collisions = 0;

    public Animator animator;
    public CapsuleCollider col;

    Vector3 moveDir;

    public Rigidbody rb;

    public Vector3 crouch;
    public Vector3 zeroX = Vector3.zero;
    public Vector3 velBeforeCrouch;
    public Vector3 StartPos = new Vector3(0f, 0.5f, -4f);

    public Vector3 ColliderStand = new Vector3();
    public Vector3 ColliderCrouch = new Vector3(0, -0.5f, 0);
    public Tile LastTile;

    public bool OnGround = true;
    public Transform MainObj;
    int environment;
    public float LastGroundY = 0;
    public Animator PlayerAnimator;

    Collider lastHit;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetSas()
    {
        StopAllCoroutines();
        transform.position = StartPos;
        CurrentX = 0;
        Collisions = 0;
        StandUp();
    }

    void Start()
    {
        environment = LayerMask.NameToLayer("Environment");
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        PlayerAnimator = GetComponent<Animator>();
        //CameraRotation = transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        if (OnGround || GameController.Instance.Rocket/* || (LastGroundY > transform.position.y && rb.velocity.y < 0)*/)
        {
            LastGroundY = transform.position.y;
        }
        else if (LastGroundY > transform.position.y && rb.velocity.y < 0)
        {
            LastGroundY = transform.position.y;
        }

        if (!OnGround && !isJumping && rb.velocity.y > 0 && !GameController.Instance.Rocket)
        {
            rb.velocity += Vector3.down * rb.velocity.y;
        }

        if (isMoving)
        {
            bool back = false;
            back = dir == -1 ? transform.position.x < CurrentX : transform.position.x > CurrentX;

            if (Mathf.Abs(transform.position.x - CurrentX) < 0.1f || back)
            {
                zeroX.Set(0, rb.velocity.y, 0);
                rb.velocity = zeroX;
                isMoving = false;
                FixPos(CurrentX);
            }
        }
    }

    //IEnumerator RotateTo(float y, float time)
    //{
    //    float rotValue = y - transform.rotation.eulerAngles.y;

    //    while (Quater)
    //}

    public void Move(bool right)
    {
        if (GameController.Instance.BlockMoving) return;

        dir = right ? 1 : -1;
        if (CurrentX != dir * Step)
        {
            if (GameController.Instance.Rocket || (OnGround && !isCrouching))
            {
                if (right)
                {
                    animator.SetTrigger("Right");
                }
                else
                {
                    animator.SetTrigger("Left");
                }
            }

            CurrentX += dir * Step;
            moveDir.x = dir * moveSpeed;
            rb.AddForce(moveDir, ForceMode.Acceleration);
            isMoving = true;
        }
    }

    public void Jump()
    {
        if (GameController.Instance.Rocket)
            return;

        if (col.height == 0.35f)
            StandUp();

        if (OnGround)
        {
            animator.SetTrigger(JumpHash);
            moveDir.x = rb.velocity.x;
            moveDir.y = JumpSpeed;
            rb.velocity = moveDir;
            isJumping = true;

            AchievementsManager.Instance.CheckAchievements("Jump");
            TasksManager.Instance.CheckTasks("Jump");
        }
    }

    public void Crouch()
    {

        if (GameController.Instance.Rocket)
            return;

        if (!OnGround)
        {
            rb.AddForce(Vector3.down * CrouchPower, ForceMode.Acceleration);
        }

        col.height = 0.35f;
        col.center = ColliderCrouch;

        StartCoroutine(WaitForCrouch());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Obstacle")
        {
            LastTile = collision.transform.parent.parent.GetComponent<Tile>();

            if (GameController.Instance.Shield)
            {
                collision.collider.enabled = false;
                GameController.Instance.Shield = false;
                GameController.Instance.ShieldTimeLeft = 0;
                LastTile.ClearObstacles();
            }
            else
            {
                lastHit = collision.collider;
                lastHit.enabled = false;
                animator.SetTrigger(DeathHash);
                GameController.Instance.Started = false;
                animator.SetBool(StartedHash, false);
                StartCoroutine(WaitDeath());
                rb.useGravity = true;
            }
        }
        else if (collision.gameObject.layer == environment)
        {
            Collisions += 1;
            OnGround = true;
            isJumping = false;
            rb.useGravity = false;
        }
    }

    IEnumerator WaitDeath()
    {
        yield return new WaitForSeconds(3);

        lastHit.enabled = true;
        GameController.Instance.FinishGame();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == environment)
        {
            Collisions -= 1;
            if (Collisions <= 0)
            {
                Collisions = 0;
                OnGround = false;

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

        isCrouching = true;
        animator.SetTrigger(CrouchHash);

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
        Canvaser.Instance.GamePanel.Shield.gameObject.SetActive(true);
        while (GameController.Instance.ShieldTimeLeft > 0 && GameController.Instance.Shield)
        {
            yield return GameController.Frame;
            GameController.Instance.ShieldTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Shield.SetTimer(GameController.Instance.ShieldTimeLeft);
        }
        Canvaser.Instance.GamePanel.Shield.gameObject.SetActive(false);
        GameController.Instance.Shield = false;
    }
}
