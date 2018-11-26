using UnityEngine;

public class BlopFollower : MonoBehaviour
{
    public Transform Target;

    private Projector projector;
    private Vector3 offset;

    void Start()
    {
        offset = Target.position - transform.position;
        projector = GetComponent<Projector>();
    }


    void Update()
    {
        if (PlayerController.Instance.OnGround)
        {
            if (!projector.enabled)
            {
                projector.enabled = true;
            }
            transform.position = Target.position - offset;
        }
        else
        {
            projector.enabled = false;
        }
    }
}
