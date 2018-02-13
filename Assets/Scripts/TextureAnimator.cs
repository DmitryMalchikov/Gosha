using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAnimator : MonoBehaviour {

	public float SpeedCoef = .5f;
	private Material mat;
	private float offset = 0;
	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameController.Instance.Started)
			return;

		offset += Time.deltaTime * GameController.Instance.Speed.z * SpeedCoef;
		offset %= 1;
		mat.SetTextureOffset ("_MainTex", new Vector2(0, offset));
	}
}
