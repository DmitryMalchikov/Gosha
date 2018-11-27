using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitParticles : MonoBehaviour
{
    private ParticleSystem _part;
    private IEnumerator Start()
    {
        _part = GetComponent<ParticleSystem>();

        while (true)
        {
            yield return new WaitForSeconds(3);
            _part.Emit(30);
        }
    }
}
