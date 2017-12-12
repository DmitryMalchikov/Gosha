using UnityEngine;

public class Car : MonoBehaviour {

    public float Speed;
    Vector3 defaultPos;
    bool firstEnable = true;

    private void OnEnable()
    {
        if (Speed > 0)
        {
            if (firstEnable)
            {
                defaultPos = transform.localPosition;
                firstEnable = false;
            }
            else
            {
                transform.localPosition = defaultPos;
            }
        }
    }

    void Update ()
    {
        if (Speed > 0 && GameController.Instance.Started)
        {
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        }
	}
}
