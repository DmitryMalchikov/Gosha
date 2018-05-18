using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithBlock : Button
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (NetworkHelper.NoRequests())
        {
            base.OnPointerClick(eventData);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (NetworkHelper.NoRequests())
        {
            base.OnPointerDown(eventData);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (NetworkHelper.NoRequests())
        {
            base.OnPointerUp(eventData);
        }
    }
}
