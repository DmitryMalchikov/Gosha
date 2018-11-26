using UnityEngine;

public class House : MonoBehaviour {

    public GameObject[] Objects;
    public GameObject[] Roofs;
    public Transform RoofsParent;
	public GameObject[] Visors;

	public MeshRenderer[] Floors;
	public Material[] FloorMaterials;

    public Vector3 SecondFloor = new Vector3(0, 60, 0);
    public Vector3 ThirdFloor = new Vector3(0, 88.9f, 0);

    private void OnEnable()
    {
        SetHouse();
    }

    public void SetHouse()
    {
        byte visorNumber = (byte)Random.Range(0, Visors.Length + 1);

        for (byte i = 0; i < Visors.Length; i++)
        {
            Visors[i].SetActive(i == visorNumber);
        }

		int FloorColor = Random.Range (0, FloorMaterials.Length);

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

		for (int i = 0; i < Floors.Length; i++) 
		{
			var materials = Floors [i].sharedMaterials;
			materials [materials.Length - 1] = FloorMaterials [FloorColor];
			Floors [i].sharedMaterials = materials;
		}

        int roofNum = Random.Range(0, Roofs.Length);

        for (int i = 0; i < Roofs.Length; i++)
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

        byte objectsCount = 0;

        for (int i = 0; i < Objects.Length && objectsCount < 3; i++)
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
