using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarObstacles : MonoBehaviour
{
    public float Speed = 4f;
    public List<Car> Cars;

    public void Start()
    {
        Cars = new List<Car>(GetComponentsInChildren<Car>());
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
        for (int i = 0; i < Cars.Count; i++)
        {
            if (Cars[i].IsMoving)
            {
                Cars[i].Speed = speed * (1 + (((GameController.Instance.Speed.z / -6f) -1f)/2f));
                if (speed == 0)
                {
                    Cars[i].ResetPos();
                }
            }
        }
    }
}
