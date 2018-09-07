using System.Collections;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
	public Transform target;
	public Vector3 offset;
	public float ZOffset;
	public float speedY = 12;
	public float speedX = 20;
    float y;
	float x;
	float z;
    public Animator animator;

	public float ShakeDuration = 1;
	public float ShakeAmount = .5f;

	private float _shakeDuration;
	private bool _shaking = false;
	private Vector3 _originalPos;

    private void OnEnable()
    {
		if (target == null) {
			target = PlayerController.Instance.transform;
		}
        offset = transform.position - target.position;
		ZOffset = offset.z;
        y = target.position.y;
		//x = transform.position.x;
        animator.enabled = false;
       
		PlayerController.Instance.rb.constraints = PlayerController.FreezeExceptJump;
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
		z = Mathf.Lerp(transform.position.z, PlayerController.Instance.transform.position.z + offset.z, speedY * Time.deltaTime);
		if (!_shaking) {
			transform.position = new Vector3 (x, y, z) ;
		} else {
			_originalPos = new Vector3 (x, y, z);
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
