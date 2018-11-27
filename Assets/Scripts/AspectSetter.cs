using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AspectSetter : MonoBehaviour
{
    public float Aspect = 1;

    private void Update()
    {
        GetComponent<Camera>().aspect = Aspect;
    }
}
