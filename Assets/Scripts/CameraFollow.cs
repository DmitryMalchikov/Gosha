using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour {

    public static CameraFollow Instance{ get; private set; }

	public Transform target;
	public Vector3 offset;
	public float speed = 12;
    float y;
    public Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        offset = transform.position - target.position;
        y = target.position.y;
        animator.enabled = false;
       
        Canvaser.Instance.GamePanel.gameObject.SetActive(true);
        ScoreManager.StartRun();
        GameController.Instance.ResetScores();
    }

    void LateUpdate ()
	{
        y = Mathf.Lerp(transform.position.y, PlayerController.Instance.LastGroundY + offset.y, speed * Time.deltaTime);
        transform.position = new Vector3(target.position.x, y, target.position.z) + offset - Vector3.up * offset.y;
	}

    public void ChangeCamera()
    {
        if (!animator.enabled)
        {
            animator.enabled = true;
        }
        animator.SetTrigger("Change");
    }
}
