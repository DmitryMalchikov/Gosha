using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HouseSetter : MonoBehaviour
{
    void Start()
    {
        var house = GetComponent<House>();
        house.RoofsParent = transform.Find("Roof");
        List<GameObject> roofs = new List<GameObject>();
        for (int i = 0; i < house.RoofsParent .childCount; i++)
        {
            roofs.Add(house.RoofsParent.GetChild(i).gameObject);
            if (house.Objects.Contains(house.RoofsParent.GetChild(i).gameObject))
            {
                house.Objects.Remove(house.RoofsParent.GetChild(i).gameObject);
            }
        }
        house.Roofs = roofs;
    }
}
