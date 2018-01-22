using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour {
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Player")) {
			PlayerController.Instance.OnRamp = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Player")) {
			PlayerController.Instance.OnRamp = false;
			PlayerController.Instance.rb.velocity += Vector3.down * PlayerController.Instance.rb.velocity.y;
		}
	}

}
