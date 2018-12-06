using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class House : MonoBehaviour
    {
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
            SetVisor();
            SetFloors();
            SetBuildingColor();
            SetRoof();
            SetAttachments();
        }

        public void SetVisor()
        {
            byte visorNumber = (byte)Random.Range(0, Visors.Length + 1);

            for (byte i = 0; i < Visors.Length; i++)
            {
                Visors[i].SetActive(i == visorNumber);
            }
        }

        public void SetRoof()
        {
            int roofNum = Random.Range(0, Roofs.Length);

            for (int i = 0; i < Roofs.Length; i++)
            {
                Roofs[i].SetActive(i == roofNum);
            }
        }

        public void SetAttachments()
        {
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

        public void SetFloors()
        {
            if (Random.Range(0, 4) == 0)
            {
                Floors[1].transform.parent.gameObject.SetActive(false);
                RoofsParent.localPosition = SecondFloor;
            }
            else
            {
                Floors[1].transform.parent.gameObject.SetActive(true);
                RoofsParent.localPosition = ThirdFloor;
            }
        }

        public void SetBuildingColor()
        {
            int FloorColor = Random.Range(0, FloorMaterials.Length);
            for (int i = 0; i < Floors.Length; i++)
            {
                var materials = Floors[i].sharedMaterials;
                materials[materials.Length - 1] = FloorMaterials[FloorColor];
                Floors[i].sharedMaterials = materials;
            }
        }
    }
}
