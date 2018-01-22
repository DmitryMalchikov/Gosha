using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Hashes
    static int CrouchHash = Animator.StringToHash("Crouch");
    static int JumpHash = Animator.StringToHash("Jump");
    static int DeathHash = Animator.StringToHash("Death");
	static int HitLeft = Animator.StringToHash("HitLeft");
	static int HitRight = Animator.StringToHash ("HitRight");
	static int Left = Animator.StringToHash("Left");
	static int Right = Animator.StringToHash ("Right");
	static int OnGroundHash = Animator.StringToHash("OnGround");
    public static int StartedHash = Animator.StringToHash("Started");
    public static int RocketHash = Animator.StringToHash("Rocket");
    #endregion

    public static PlayerController Instance;

	public bool UseHardTouch = true;
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

	public Transform AnimatorRoot;
    public Animator animator;
    public CapsuleCollider col;
	public float MaxCollisionAngle;
	public float MinCollisionAngle;
	public int MaxHitsCount = 2;
	public float DeltaXOffset = .1f;

    private Vector3 moveDir;

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

	private string ObstacleTag = "Obstacle";
    private float minY = 0;

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

    int environment;
	int environmentMask;
    public float LastGroundY = 0;
    public Animator PlayerAnimator;
    public Transform Position;

    private Collider lastHit;
	private Vector3 velocityBeforePhysics;
	private byte hitsCount;

    private void Awake()
    {
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;
        Instance = this;
    }

    public void ResetSas()
    {
        StopAllCoroutines();
        transform.position = StartPos;
		animator.transform.rotation = new Quaternion ();
		animator.SetTrigger ("Reset");
        OnGround = true;
        CurrentX = 0;
        Collisions = 0;
		hitsCount = 0;
		//rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        StandUp();
    }

    void Start()
    {
        environment = LayerMask.NameToLayer("Environment");
		environmentMask = LayerMask.GetMask ("Environment");
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        PlayerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (OnGround || GameController.Instance.Rocket)
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

		if ((!OnGround || !tempOnGround) && rb.velocity.y > 0)
		{
			//Debug.Log ("Down");
			StickToGround ();
			//rb.velocity += Vector3.down * rb.velocity.y;
		}

        if (isMoving)
        {
            bool back = false;
            back = dir == -1 ? transform.position.x < CurrentX : transform.position.x > CurrentX;

			if (Mathf.Abs (transform.position.x - CurrentX) < 0.1f || back) {
				zeroX.Set (0, rb.velocity.y, 0);
				rb.velocity = zeroX;
				moveDir = Vector3.zero;
				isMoving = false;
				FixPos (CurrentX);
			}
        }
    }

	void FixedUpdate()
	{
		velocityBeforePhysics = rb.velocity;
	}

	public void StickToGround(){
		if (!isJumping && !GameController.Instance.Rocket)
		{
		RaycastHit hit;
			if (Physics.Raycast (new Ray (transform.position + Vector3.up, Vector3.down), out hit, 1.5f, environmentMask)) {
				transform.position = hit.point + Vector3.up * .4f;
				rb.velocity += Vector3.down * rb.velocity.y;
				//OnGround = true;
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

			rb.velocity += new Vector3 (-rb.velocity.x, 0, 0);
            CurrentX += dir * Step;
            moveDir.x = dir * moveSpeed;
            if (OnGround)
            {
                moveDir.y = -100;
            }
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

			AchievementsManager.Instance.CheckAchievements(TasksTypes.Jump);
			TasksManager.Instance.CheckTasks(TasksTypes.Jump);
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

        isCrouching = true;
		col.center = ColliderCrouch;
        col.height = 0.35f;
        
        animator.SetTrigger(CrouchHash);

        StartCoroutine(WaitForCrouch());
    }

    private void OnCollisionEnter(Collision collision)
    {
		if (collision.transform.tag == ObstacleTag)
        {
			if (!GameController.Instance.Started)
				return;

			LastTile = collision.transform.GetComponentInParent<Tile>();

			if (GameController.Instance.Shield)
			{
				LastTile.DisableCollider (collision.collider);
				//collision.collider.enabled = false;
				GameController.Instance.Shield = false;
				GameController.Instance.ShieldTimeLeft = 0;
				LastTile.ClearObstacles();
				LastTile = null;
				rb.velocity = velocityBeforePhysics;
				return;
			}

				Vector3 normal = collision.contacts [0].normal;
				Vector2 normal2 = new Vector2 (normal.x, normal.z);
			float pointDelta = transform.position.x - collision.contacts [0].point.x;
				//float angle = Vector3.Angle (normal, Vector3.forward);
			float angle = Vector2.Angle(normal2, Vector2.up);
			bool sidehit = (normal2 != Vector2.zero && angle > MinCollisionAngle && angle < MaxCollisionAngle);
			bool hardContact = Mathf.Abs (pointDelta) > DeltaXOffset;
			if (hitsCount < MaxHitsCount) {
				if (sidehit) {
					dir = Mathf.Sign (normal.x);
					if (OnGround) {
						if (dir == 1) {
							animator.SetTrigger (HitLeft);
						} else {
							animator.SetTrigger (HitRight);
						}
					}
					if (CurrentX != dir * Step) {
						CurrentX += dir * Step;
					}
					moveDir.x = dir * moveSpeed/2;
					if (OnGround) {
						moveDir.y = -100;
					}
					rb.velocity += new Vector3 (-rb.velocity.x, 0, 0);
					rb.AddForce (moveDir, ForceMode.Acceleration);
					isMoving = true;
					hitsCount++;
					CameraFollow.Instance.ShakeCamera ();
					return;
				} else if (hardContact && UseHardTouch) {					
					dir = Mathf.Sign (pointDelta);
					if (OnGround) {
						if (dir == 1) {
							animator.SetTrigger (HitLeft);
						} else {
							animator.SetTrigger (HitRight);
						}
					}
					LastTile.DisableCollider (collision.collider);
					LastTile = null;
					rb.velocity = velocityBeforePhysics;
					hitsCount++;
					return;
				}
				}

//			if (sidehit) {
//				animator.transform.rotation = Quaternion.LookRotation (Quaternion.Euler (0, DeathRotation * dir, 0) * Vector3.forward);
//			}
			animator.transform.rotation = Quaternion.Euler(0, AnimatorRoot.rotation.eulerAngles.y - 90, 0);
                lastHit = collision.collider;
			//rb.constraints = RigidbodyConstraints.FreezeRotation;
                lastHit.enabled = false;
                animator.SetTrigger(DeathHash);
                GameController.Instance.Started = false;
                animator.SetBool(StartedHash, false);
                StartCoroutine(WaitDeath());
                rb.useGravity = true;
			rb.velocity += Vector3.left * rb.velocity.x + Vector3.down * rb.velocity.y;
			animator.ResetTrigger (HitLeft);
			animator.ResetTrigger (HitRight);
			animator.ResetTrigger (Left);
			animator.ResetTrigger (Right);
        }
        else if (collision.gameObject.layer == environment)
        {
            if (minY == 0)
            {
                minY = transform.position.y;
            }

            Collisions += 1;
            OnGround = true;
            isJumping = false;
            rb.useGravity = false;
        }
    }

    IEnumerator WaitDeath()
    {
        yield return new WaitForSeconds(1.8f);

        lastHit.enabled = true;
        GameController.Instance.FinishGame();
    }

	bool tempOnGround = true;

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == environment)
        {
			tempOnGround = false;
			StartCoroutine (SetOnGround ());
        }
    }

	IEnumerator SetOnGround(){
		yield return new WaitForFixedUpdate();

		if (!tempOnGround) {
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
