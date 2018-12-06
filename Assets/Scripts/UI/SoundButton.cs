using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
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
}
