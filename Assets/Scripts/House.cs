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
            Floors[2].transform.parent.gameObject.SetActive(false);
            Roofs.transform.localPosition = SecondFloor;
        }
        else
        {
            Floors[2].transform.parent.gameObject.SetActive(true);
            Roofs.transform.localPosition = ThirdFloor;
        }

		for (int i = 0; i < Floors.Count; i++) 
		{
			Floors [i].material = FloorMaterials [FloorColor];
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
