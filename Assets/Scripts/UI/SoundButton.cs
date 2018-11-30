using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour, IPointerClickHandler
{
    private Selectable _selectable;

    private void Start()
    {
        _selectable = GetComponent<Selectable>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_selectable.interactable)
        {
            AudioManager.PlayBtnTapSound();
        }
    }
}
