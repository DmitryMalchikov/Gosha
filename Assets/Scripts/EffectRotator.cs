using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRotator : MonoBehaviour {

	public float RotationSpeed = 40;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.forward, RotationSpeed * Time.deltaTime);
	}
}
