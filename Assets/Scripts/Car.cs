using UnityEngine;

public class Car : MonoBehaviour {

    public float Speed;
    Vector3 defaultPos;
    bool firstEnable = true;
    public bool IsMoving = true;

    private void OnEnable()
    {
        if (IsMoving)
        {
            if (firstEnable)
            {
                defaultPos = transform.localPosition;
                firstEnable = false;
            }
            else
            {
                ResetPos();
            }
        }
    }

    public void ResetPos()
    {
        transform.localPosition = defaultPos;
    }

    void Update ()
    {
        if (Speed > 0 && GameController.Instance.Started)
        {
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        }
	}
}
