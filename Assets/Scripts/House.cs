using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

    public List<GameObject> Objects;
    public GameObject Roofs;

	public MeshRenderer Visor;
	public List<Material> VisorMaterials;

	public List<MeshRenderer> Floors;
	public List<Material> FloorMaterials;

    public Vector3 SecondFloor = new Vector3(0, 60, 0);
    public Vector3 ThirdFloor = new Vector3(0, 88.9f, 0);

	private List<Material> InstMaterials = new List<Material>();

    private void OnEnable()
    {
        SetHouse();
    }

//	private void OnDisable(){
//		for (int i = 0; i < InstMaterials.Count; i++) {
//			Destroy (InstMaterials [i]);
//		}
//		InstMaterials.Clear ();
//	}

    public void SetHouse()
    {
		Visor.material = VisorMaterials[Random.Range (0, VisorMaterials.Count)];

		int FloorColor = Random.Range (0, FloorMaterials.Count);

        if (Random.Range(0,4) == 0)
        {
            Floors[1].transform.parent.gameObject.SetActive(false);
            Roofs.transform.localPosition = SecondFloor;
        }
        else
        {
            Floors[1].transform.parent.gameObject.SetActive(true);
            Roofs.transform.localPosition = ThirdFloor;
        }

		for (int i = 0; i < Floors.Count; i++) 
		{
			var materials = Floors [i].sharedMaterials;
			materials [materials.Length - 1] = FloorMaterials [FloorColor];
			Floors [i].sharedMaterials = materials;
			//InstMaterials.AddRange (materials);
		}

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Random.Range(0, 4) == 0)
            {
                Objects[i].SetActive(true);
            }
            else
            {
                Objects[i].SetActive(false);
            }

        }
    }
}
