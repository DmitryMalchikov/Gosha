using System.Collections;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public float Step = 2;
    [Range(0, 1)]
    public float GravityOnPercent = .75f;
    public float LowDelta = 1.5f; 
    public float moveSpeed = 1;   
    public bool isMoving;
    public float MaxCollisionAngle;
    public float MinCollisionAngle;
    public int MaxHitsCount = 2;
    public Vector3 StartPos = new Vector3(0f, 0.5f, -4f);

    [HideInInspector]
    public Tile LastTile;

    [Header("Effects")]
    public ParticleSystem ObstaclesEffect;

    private Vector3 _moveDir;
    private float _fallDistance;
    private float _dir;    
    private float _minY = 0;
    private float _currentX = 0;
    private bool _isJumping = false;

    public bool OnGround
    {
        get { return _onGround; }
        set
        {
            _onGround = value;
            PlayerAnimator.SetOnGround(_onGround);
        }
    }
    bool _onGround = true;
    
    public float LastGroundY = 0;
    public Animator CurrentAnimator;

    private Collider lastHit;
    private byte hitsCount;

    public void ResetSas()
    {
        StopAllCoroutines();
        PlayerCollider.ResetCollider();
        PlayerRigidbody.ResetRigidbody();
        CurrentAnimator.SetTrigger("Change");
        PlayerCollisions.ClearCollisionsExceptGround();
        transform.position = StartPos;
        OnGround = true;
        _currentX = 0;
        hitsCount = 0;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        _fallDistance = Step * (1f - GravityOnPercent);
        CurrentAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if ((OnGround && tempOnGround) || PlayerRocket.RocketInProgress)
        {
            LastGroundY = transform.position.y;
        }
        else if (LastGroundY > transform.position.y && PlayerRigidbody.Velocity.y < 0)
        {
            if (transform.position.y >= _minY)
            {
                LastGroundY = transform.position.y;
            }
        }

        if (!GameController.Started)
            return;

        if ((!OnGround && PlayerRigidbody.Velocity.y > 0 && !isMoving) && !Ramp.PlayerOnRamp)
        {
            StickToGround();
        }

        if (isMoving)
        {
            bool back = _dir == -1 ? transform.position.x < _currentX : transform.position.x > _currentX;
            float movedDistance = Mathf.Abs(transform.position.x - _currentX);

            if (!PlayerRigidbody.UseGravity && movedDistance < _fallDistance && !PlayerRocket.RocketInProgress)
            {
                if (!PlayerCollisions.AnyCollisions)
                {
                    PlayerRigidbody.SetInAir();
                }
            }

            if (movedDistance < 0.01f || back)
            {
                _moveDir = Vector3.zero;
                PlayerRigidbody.FreezeExceptJump();
                isMoving = false;
                FixPos(_currentX);
                StartCoroutine(SetOnGround());
            }
            else
            {
                transform.Translate(_moveDir * moveSpeed * Time.deltaTime);
            }
        }
    }

    void LateUpdate()
    {
        Ramp.StickPlayerToGround();
    }

    public void StickToGround()
    {
        if (!_isJumping && !PlayerRocket.RocketInProgress)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), out hit, 1.5f, Masks.EnvironmentMask))
            {
                transform.position = hit.point + Vector3.up * .4f;
                PlayerRigidbody.ResetVelocity();
                tempOnGround = true;
                OnGround = true;
            }
        }
    }

    public void Move(bool right)
    {
        if (PlayerRocket.BlockMoving) return;

        _dir = right ? 1 : -1;
        if (_currentX != _dir * Step)
        {
            if (PlayerRocket.RocketInProgress || OnGround)
            {
                PlayerAnimator.SetTurnTrigger(right);
            }

            if (OnGround)
            {
                PlayerRigidbody.FreezeAll();
            }

            if (!PlayerRocket.RocketInProgress)
            {
                AudioManager.PlaySideMove();
            }

            SetPositionBeforeMove();

            _currentX += _dir * Step;
            _moveDir = Vector3.right * _dir;
            PlayerCollider.StandUp();
            isMoving = true;
        }
    }

    public void Jump()
    {
        if (PlayerRocket.RocketInProgress)
            return;

        StartCoroutine(StartJump());
    }

    IEnumerator StartJump()
    {
        yield return new WaitUntil(() => isMoving == false && tempOnGround == true);

        if (PlayerCollider.IsCrouch)
            PlayerCollider.StandUp();

        if (OnGround)
        {
            PlayerAnimator.SetJumpTrigger();
            PlayerRigidbody.Jump();
            _isJumping = true;
            tempOnGround = false;

            AudioManager.PlayJump();
            AchievementsManager.Instance.CheckAchievements(TasksTypes.Jump);
            TasksManager.Instance.CheckTasks(TasksTypes.Jump);
        }
    }

    public void Crouch()
    {
        if (PlayerRocket.RocketInProgress || !GameController.Started || PlayerCollider.IsCrouch)
            return;

        PlayerRigidbody.MoveToGround(OnGround);        
        PlayerCollider.Crouch();
        PlayerAnimator.SetCrouchTrigger();

        StartCoroutine(WaitForCrouch());
    }

    private void OnSideHit(Vector3 normal, Collision collision)
    {
        _dir = Mathf.Sign(normal.x);
        if (OnGround)
        {
            PlayerAnimator.SetSideHitTrigger(_dir);
        }
        if (_currentX != _dir * Step)
        {
            _currentX += _dir * Step;
        }

        _moveDir = Vector3.right * _dir;
        PlayerRigidbody.FreezeAll();
        isMoving = true;

        hitsCount++;
        CameraFollow.Instance.ShakeCamera();
        AudioManager.PlaySideHit();
    }

    private void OnHit(Collision collision)
    {
        lastHit = collision.collider;
        lastHit.enabled = false;
        PlayerAnimator.SetDeath();
        PlayerRigidbody.OnHit();
        CameraFollow.OnHit();
        Canvaser.Instance.GamePanel.TurdOffBonuses();
        GameController.OnHit();
        AudioManager.PlayHit();
        StartCoroutine(WaitDeath());
    }

    private void OnGrounded(Collision collision)
    {
        if (_minY == 0)
        {
            _minY = transform.position.y;
        }

        PlayerCollisions.AddCollision(collision.gameObject);
        OnGround = true;
        tempOnGround = true;
        _isJumping = false;
        PlayerRigidbody.UseGravity = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != Masks.DefaultLayer && collision.gameObject.layer != Masks.GroundLayer)
        {
            return;
        }

        if (collision.gameObject.CompareTag("HardObstacle"))
        {
            if (PlayerShield.ShieldIsOn)
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
            if (PlayerShield.ShieldIsOn)
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
        }
        else
        {
            if (PlayerShield.ShieldIsOn)
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
        EffectsManager.TurnOffEffects();
        yield return new WaitForSeconds(1.8f);

        lastHit.enabled = true;
        PlayerCollider.ColliderEnabled = false;
        PlayerRigidbody.FreezeAll();
        GameController.Instance.FinishGame();
    }

    bool tempOnGround = true;

    private void ShieldHit()
    {
        PlayerShield.OnHit();
        RemoveObstcles();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != Masks.DefaultLayer && collision.gameObject.layer != Masks.GroundLayer)
        {
            return;
        }

        if (PlayerCollisions.RemoveCollision(collision.gameObject))
        {
            tempOnGround = false;
            StartCoroutine(SetOnGround());
        }
    }

    private void SetPositionBeforeMove()
    {
        if (PlayerRocket.RocketInProgress || PlayerCollisions.AnyCollisions)
            return;

        if (!_isJumping)
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
            if (!PlayerCollisions.AnyCollisions)
            {
                OnGround = false;
                tempOnGround = true;

                if (!PlayerRocket.RocketInProgress)
                {
                    PlayerRigidbody.SetInAir();
                }
            }
            else
            {
                PlayerRigidbody.ResetRigidbody();
            }
        }
    }

    IEnumerator WaitForCrouch()
    {
        yield return new WaitUntil(() => OnGround == true);
        yield return new WaitForSeconds(0.8f);
        PlayerCollider.StandUp();
    }

    public void FixPos(float posX)
    {
        transform.position = new Vector3(posX, transform.position.y, transform.position.z);
    }

    public void RemoveObstcles()
    {
        ObstaclesEffect.Play();
        StartCoroutine(WaitEffect());
    }

    public static void ResetPositionForContinue()
    {
        PlayerAnimator.Continue(true);
        Instance.transform.position += Vector3.right * (Instance._currentX - Instance.transform.position.x);
    }

    IEnumerator WaitEffect()
    {
        yield return CoroutineManager.Frame;

        var obstacles = Physics.OverlapBox(transform.position + Vector3.forward * 7f, new Vector3(4f, 3f, 8.5f), Quaternion.identity, Masks.ObstaclesAndPickables);
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].transform.parent.gameObject.SetActive(false);
        }
        ResetPositionForContinue();
        PlayerRigidbody.FreezeExceptJump();
        PlayerCollider.ColliderEnabled = true;
    }
}


