using System.Collections;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
    private Transform _target;
    private static Vector3 _offset;
    private static float _zOffset;
    public float speedY = 12;
    public float speedX = 20;
    private float y;
    private float x;
    private float z;
    public Animator animator;

    public float ShakeDuration = 1;
    public float ShakeAmount = .5f;

    private float _shakeDuration;
    private bool _shaking = false;
    private Vector3 _originalPos;

    private void OnEnable()
    {
        if (_target == null)
        {
            _target = PlayerController.Instance.transform;
        }
        _offset = transform.position - _target.position;
        _zOffset = _offset.z;
        y = _target.position.y;
        //x = transform.position.x;
        animator.enabled = false;

        PlayerRigidbody.FreezeExceptJump();
        Canvaser.Instance.GamePanel.gameObject.SetActive(true);
        ScoreManager.StartRun();
        GameController.Instance.ResetScores();
    }

    void LateUpdate()
    {
        y = Mathf.Lerp(transform.position.y, PlayerController.Instance.LastGroundY + _offset.y, speedY * Time.deltaTime);
        x = Mathf.Lerp(transform.position.x, _target.position.x / 1.15f + _offset.x, speedX * Time.deltaTime);
        z = Mathf.Lerp(transform.position.z, _target.position.z + _offset.z, speedY * Time.deltaTime);
        if (!_shaking)
        {
            transform.position = new Vector3(x, y, z);
        }
        else
        {
            _originalPos = new Vector3(x, y, z);
        }
    }

    public void ChangeCamera()
    {
        if (!animator.enabled)
        {
            animator.enabled = true;
        }
        animator.SetTrigger("Change");
    }

    public void ShakeCamera()
    {
        _shakeDuration = ShakeDuration;
        StartCoroutine(Shake());
    }

    public static void OnHit()
    {
        _offset.z = -3.2f;
    }

    public static void ResetOffset()
    {
        _offset.z = _zOffset;
    }

    IEnumerator Shake()
    {
        _shaking = true;

        while (_shakeDuration > 0)
        {
            yield return new WaitForEndOfFrame();
            transform.position = _originalPos + Random.insideUnitSphere * ShakeAmount;
            _shakeDuration -= Time.deltaTime;
        }

        _shaking = false;
    }
}
