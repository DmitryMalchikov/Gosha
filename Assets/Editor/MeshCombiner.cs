using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MeshCombiner : MonoBehaviour {

	static int counter = 1;

	public bool mesh = false;

	public List<MeshFilter> meshes = new List<MeshFilter> ();
	public List<MeshFilter> meshesWithSubmeshes = new List<MeshFilter> ();
	
	// Update is called once per frame
	void Update () {
		if (mesh == true) {
			CombineInstance[] comb = new CombineInstance[meshes.Count];
			CombineInstance[] combSubmeshes = new CombineInstance[meshesWithSubmeshes.Count + 1];
			for (int i = 0; i < comb.Length; i++)
			{ 
				comb [i].mesh = meshes [i].sharedMesh;
				comb [i].transform = meshes [i].transform.localToWorldMatrix;
			}

			for (int i = 0; i < combSubmeshes.Length - 1; i++)
			{ 
				combSubmeshes [i].mesh = meshesWithSubmeshes [i].sharedMesh;
				combSubmeshes [i].transform = meshesWithSubmeshes [i].transform.localToWorldMatrix;
			}

			Mesh result = new Mesh ();
//			Mesh tresult = new Mesh ();
			result.CombineMeshes (comb);
//			combSubmeshes [combSubmeshes.Length - 1].mesh = tresult;
//			combSubmeshes [combSubmeshes.Length - 1].transform = transform.localToWorldMatrix;
//			result.CombineMeshes (combSubmeshes, false);
			//mesh1.mesh = result;
			AssetDatabase.CreateAsset (result, "Assets/NewMeshes/Mesh" + counter + ".asset");
			AssetDatabase.SaveAssets ();
			mesh = false;
			counter++;
		}
	}
}
