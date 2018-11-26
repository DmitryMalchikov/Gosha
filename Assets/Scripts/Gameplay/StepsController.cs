using System.Collections;
using UnityEngine;

public class StepsController : MonoBehaviour
{
    public AudioClip Step;
    public float VolumeScale = 0.8f;

    private AudioSource _source;
    private WaitForSeconds _stepWait;

    void Start()
    {
        _source = GetComponent<AudioSource>();
        _stepWait = new WaitForSeconds(Step.length);
        StartCoroutine(PlayStep());
    }

    IEnumerator PlayStep()
    {
        while (true)
        {
            yield return null;
            if (GameController.Started && PlayerController.Instance.OnGround && !PlayerCollider.IsCrouch)
            {
                _source.PlayOneShot(Step, VolumeScale);
                yield return _stepWait;
            }
        }
    }
}
