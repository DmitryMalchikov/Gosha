using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Test
{
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
}
