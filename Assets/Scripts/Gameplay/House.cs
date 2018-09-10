using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

    public List<GameObject> Objects;
    public List<GameObject> Roofs;
    public Transform RoofsParent;

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

    public void SetHouse()
    {
		Visor.material = VisorMaterials[Random.Range (0, VisorMaterials.Count)];

		int FloorColor = Random.Range (0, FloorMaterials.Count);

        if (Random.Range(0,4) == 0)
        {
            Floors[1].transform.parent.gameObject.SetActive(false);
            RoofsParent.localPosition = SecondFloor;
        }
        else
        {
            Floors[1].transform.parent.gameObject.SetActive(true);
            RoofsParent.localPosition = ThirdFloor;
        }

		for (int i = 0; i < Floors.Count; i++) 
		{
			var materials = Floors [i].sharedMaterials;
			materials [materials.Length - 1] = FloorMaterials [FloorColor];
			Floors [i].sharedMaterials = materials;
		}

        int roofNum = Random.Range(0, Roofs.Count);

        for (int i = 0; i < Roofs.Count; i++)
        {
            if (i == roofNum)
            {
                Roofs[i].SetActive(true);
            }
            else
            {
                Roofs[i].SetActive(false);
            }
        }

        int objectsCount = 0;

        for (int i = 0; i < Objects.Count && objectsCount < 3; i++)
        {
            if (Random.Range(0, 4) == 0)
            {
                Objects[i].SetActive(true);
                objectsCount++;
            }
            else
            {
                Objects[i].SetActive(false);
            }
        }
    }
}
