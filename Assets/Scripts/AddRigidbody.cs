using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AddRigidbody : MonoBehaviour {
	
	public bool update = false;
	// Update is called once per frame
	void Update () {
		var tile = GetComponent<Tile> ();
		//tile.rb = GetComponent<Rigidbody> ();
		update = false;
	}
}
