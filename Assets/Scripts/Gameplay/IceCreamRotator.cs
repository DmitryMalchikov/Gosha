using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class IceCreamRotator : Singleton<IceCreamRotator>
    {
        public Animator animator;
        public float AngleDelta;

        private Vector3 _previousRot;

        public static void SetRotator(bool started)
        {
            Instance.animator.SetBool("Started", started);
        }

        void OnTriggerEnter(Collider other)
        {
            _previousRot = _previousRot - Vector3.up * AngleDelta;
            if (_previousRot.y <= -360)
            {
                _previousRot.y %= 360;
            }
            other.GetComponentInChildren<Spinner>().StartRotation(_previousRot);
        }
    }
}
