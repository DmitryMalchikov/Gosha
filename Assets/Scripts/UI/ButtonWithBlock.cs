using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ButtonWithBlock : Button
    {
        protected override void Start()
        {
            base.Start();
            ColorBlock colBlock = colors;
            colBlock.disabledColor = colBlock.normalColor;

            colors = colBlock;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
            {
                interactable = false;
            }
            else
            { 
                base.OnPointerDown(eventData);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (interactable)
            {
                base.OnPointerClick(eventData);
            }

            interactable = true;
        }
    }
}
