using UnityEngine;
using UnityEngine.UI;

public class RegionPanel : MonoBehaviour
{
    public RegionModel info;
    public LocalizedText RegionName;
    public Toggle ToggleBox;
    public Image Flag;

    public void ChooseRegion(bool isChosen)
    {
        if (isChosen)
        {
            Canvaser.Instance.RegistrationPanel.Region = info;
            Canvaser.Instance.RegistrationPanel.Phone.placeholder.GetComponent<Text>().text = info.PhonePlaceholder;
        }
    }

    public void SetRegionPanel(RegionModel model)
    {
        info = model;
        RegionName.Key = info.Name;
        Flag.sprite = Resources.Load<Sprite>("Flag" + info.Id);
    }
}
