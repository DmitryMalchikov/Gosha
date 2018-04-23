using UnityEngine;

public class WorkerAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        _animator.enabled = false;
    }

    private void OnEnable()
    {
		if (!_animator) {
			_animator = GetComponent<Animator> ();
		}
        _animator.enabled = true;
    }
}
