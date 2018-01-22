using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour {

    public static CameraFollow Instance{ get; private set; }

	public Transform target;
	public Vector3 offset;
	public float speedY = 12;
	public float speedX = 20;
    float y;
	float x;
    public Animator animator;

	public float ShakeDuration = 1;
	public float ShakeAmount = .5f;

	private float _shakeDuration;
	private bool _shaking = false;
	private Vector3 _originalPos;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
		if (target == null) {
			target = PlayerController.Instance.transform;
		}
        offset = transform.position - target.position;
        y = target.position.y;
		//x = transform.position.x;
        animator.enabled = false;
       
        Canvaser.Instance.GamePanel.gameObject.SetActive(true);
        ScoreManager.StartRun();
        GameController.Instance.ResetScores();
    }

    void LateUpdate ()
	{
		if (Input.GetKeyDown (KeyCode.P)) {
			ShakeCamera ();
		}

        y = Mathf.Lerp(transform.position.y, PlayerController.Instance.LastGroundY + offset.y, speedY * Time.deltaTime);
		x = Mathf.Lerp(transform.position.x, PlayerController.Instance.transform.position.x/1.15f + offset.x, speedX * Time.deltaTime);
		if (!_shaking) {
			transform.position = new Vector3 (x, y, target.position.z) + offset - Vector3.up * offset.y;
		} else {
			_originalPos = new Vector3 (x, y, target.position.z) + offset - Vector3.up * offset.y;
		}

		//transform.LookAt (target);
	}

    public void ChangeCamera()
    {
        if (!animator.enabled)
        {
            animator.enabled = true;
        }
        animator.SetTrigger("Change");
    }

	public void ShakeCamera(){
		_shakeDuration = ShakeDuration;
		StartCoroutine (Shake ());
	}

	IEnumerator Shake(){
		_shaking = true;

		while (_shakeDuration > 0) {
			yield return new WaitForEndOfFrame ();
			transform.position = _originalPos + Random.insideUnitSphere * ShakeAmount;
			_shakeDuration -= Time.deltaTime;
		}

		_shaking = false;
	}
}
