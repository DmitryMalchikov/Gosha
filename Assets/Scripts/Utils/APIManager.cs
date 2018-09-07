using UnityEngine;

public abstract class APIManager<T> : Singleton<T> where T: MonoBehaviour
{
	protected virtual void Start ()
    {
        SetUrls();
	}

    public abstract void SetUrls();
}
