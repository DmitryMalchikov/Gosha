using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {
    public Vector3 Rotatation = new Vector3(0 ,10f ,0);

    void Update()
    {
			transform.Rotate (Rotatation * Time.deltaTime, Space.World);
    }

	public void StartRotation(Vector3 previousRot){
		transform.rotation = Quaternion.Euler (previousRot);
	}
}
