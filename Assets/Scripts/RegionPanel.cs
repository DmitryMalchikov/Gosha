using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionPanel : MonoBehaviour {

    public RegionModel info;
    public Text RegionName;
    public Toggle ToggleBox;

    public void ChooseRegion(bool isChosen)
    {
        if(isChosen)
        {
            Canvaser.Instance.RegistrationPanel.Region = info;
        }
    }

    public void SetRegionPanel(RegionModel model)
    {
        info = model;
        RegionName.text = info.Name;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
