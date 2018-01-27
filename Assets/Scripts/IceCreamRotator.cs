﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreamRotator : MonoBehaviour {

	public static IceCreamRotator Instance{ get; private set;}

	public Animator animator;
	public float AngleDelta;

	private int pickableLayer;
	private Vector3 previousRot;

	void Awake(){
		Instance = this;	
	}

	public static void SetRotator(bool started){
		Instance.animator.SetBool ("Started", started);
	}

	void Start(){
		pickableLayer = LayerMask.NameToLayer ("Pickable");
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.layer == pickableLayer) {
				previousRot = previousRot - Vector3.up * AngleDelta;
				if (previousRot.y <= -360) {
					previousRot.y %= 360;
				}
				other.GetComponentInChildren<Spinner> ().StartRotation (previousRot);
		}
	}
}
