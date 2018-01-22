using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TexturePacker : MonoBehaviour {
	static int counter = 1;

	public bool pack;
	public Texture2D[] textures;
	
	// Update is called once per frame
	void Update () {
		if (pack) {
			Texture2D atlas = new Texture2D (4096, 4096);
			atlas.PackTextures (textures, 2,4096);
			AssetDatabase.CreateAsset (atlas, "Assets/NewTextures/Text" + counter + ".asset");
			AssetDatabase.SaveAssets ();
			pack = false;
		}
	}
}
