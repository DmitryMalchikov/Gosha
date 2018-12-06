using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Car : MonoBehaviour
    {
        public float Speed;
        Vector3 _defaultPos;
        private bool _firstEnable = true;
        public bool IsMoving = true;

        private void OnEnable()
        {
            if (!IsMoving) return;

            if (_firstEnable)
            {
                _defaultPos = transform.localPosition;
                _firstEnable = false;
            }
            else
            {
                ResetPos();
            }
        }

        public void ResetPos()
        {
            transform.localPosition = _defaultPos;
        }

        void Update()
        {
            if (Speed > 0 && GameController.Started)
            {
                transform.Translate(Vector3.forward * Speed * (-SpeedController.Speed.z) * Time.deltaTime);
            }
        }
    }
}
