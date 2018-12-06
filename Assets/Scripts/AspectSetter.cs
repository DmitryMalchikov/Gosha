using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class AspectSetter : MonoBehaviour
    {
        public float Aspect = 1;

        private void Update()
        {
            GetComponent<Camera>().aspect = Aspect;
        }
    }
}
