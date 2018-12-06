using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Spinner : MonoBehaviour
    {
        public Vector3 Rotation = new Vector3(0, 60f, 0);

        void Update()
        {
            transform.Rotate(Rotation * Time.deltaTime, Space.World);
        }

        public void StartRotation(Vector3 previousRot)
        {
            transform.rotation = Quaternion.Euler(previousRot);
        }
    }
}
