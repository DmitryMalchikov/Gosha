using System;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class CarObstacles : MonoBehaviour
    {
        public float Speed = 4f;
        public Car[] Cars;

        public void Start()
        {
            Cars = GetComponentsInChildren<Car>();
            StopCars();
        }

        public void StopCars()
        {
            SetCarsSpeed(0);
        }
        public void StartCars()
        {
            SetCarsSpeed(Speed);
        }

        public void SetCarsSpeed(float speed)
        {
            for (int i = 0; i < Cars.Length; i++)
            {
                if (!Cars[i].IsMoving) continue;

                Cars[i].Speed = speed * (1 + (((SpeedController.StartSpeed / -6f) -1f)/2f));
                Cars[i].Speed = Mathf.Abs((float)Cars[i].Speed / (float)SpeedController.StartSpeed);
                if (Mathf.Abs(speed) < 0.1f)
                {
                    Cars[i].ResetPos();
                }
            }
        }
    }
}
