using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsController : MonoBehaviour
{

    public AudioClip Step;
    public float VolumeScale = 0.8f;

    private AudioSource _source;

    void Start()
    {
        _source = GetComponent<AudioSource>();
        StartCoroutine(PlayStep());
    }

    IEnumerator PlayStep()
    {
        while (true)
        {
            yield return null;
            if (GameController.Instance.Started && PlayerController.Instance.OnGround && !PlayerController.Instance.isCrouching)
            {
                _source.PlayOneShot(Step, VolumeScale);
                yield return new WaitForSeconds(Step.length);
            }
        }
    }
}
