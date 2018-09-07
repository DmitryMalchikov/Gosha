using UnityEngine;

public class IceCreamRotator : Singleton<IceCreamRotator>
{
    public Animator animator;
    public float AngleDelta;

    private int pickableLayer;
    private Vector3 previousRot;

    public static void SetRotator(bool started)
    {
        Instance.animator.SetBool("Started", started);
    }

    void Start()
    {
        pickableLayer = LayerMask.NameToLayer("Pickable");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == pickableLayer)
        {
            previousRot = previousRot - Vector3.up * AngleDelta;
            if (previousRot.y <= -360)
            {
                previousRot.y %= 360;
            }
            other.GetComponentInChildren<Spinner>().StartRotation(previousRot);
        }
    }
}
