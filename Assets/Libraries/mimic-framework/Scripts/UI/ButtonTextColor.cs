using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Mimic.UI {

    public class ButtonTextColor : Button {

        [SerializeField]
        private Color normalColor = Color.white;
        public Color NormalColor {
            get => normalColor;
            set => normalColor = value;
        }

        [SerializeField]
        private Color highlightedColor = Color.white;
        public Color HighlightedColor {
            get => highlightedColor;
            set => highlightedColor = value;
        }

        [SerializeField]
        private Color pressedColor = Color.white;
        public Color PressedColor  {
            get => pressedColor; 
            set => pressedColor = value; 
        }

        [SerializeField]
        private Color disabledColor = Color.white;
        public Color DisabledColor {
            get => disabledColor;
            set => disabledColor = value;
        }

        private bool isHighlighted;
        private bool isPressed;  

        private List<Text> texts;

        protected override void Awake() {
            base.Awake();
            texts = new List<Text>(GetComponentsInChildren<Text>());
        }
        
        public override void OnPointerDown(PointerEventData eventData) {
            if (interactable) {
                isPressed = true;
                texts.ForEach(t => t.color = PressedColor);
                base.OnPointerDown(eventData);
            }
        }

        public override void OnPointerUp(PointerEventData eventData) {
            if (interactable) {
                isPressed = false;
                texts.ForEach(t => t.color = isHighlighted ? HighlightedColor : NormalColor);
                base.OnPointerUp(eventData);
            }
        }
        
        public override void OnPointerExit(PointerEventData eventData) {
            if (interactable) {
                isHighlighted = false;
                if(isPressed)
                    texts.ForEach(t => t.color = interactable ? PressedColor : DisabledColor);
                else
                    texts.ForEach(t => t.color = interactable ? NormalColor : DisabledColor);
                base.OnPointerExit(eventData);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData) {
            if (interactable) {
                isHighlighted = true;
                if (isPressed) {
                    texts.ForEach(t => t.color = interactable ? PressedColor : DisabledColor);
                } else {
                    texts.ForEach(t => t.color = interactable ? HighlightedColor : DisabledColor);
                }
                base.OnPointerEnter(eventData);
            }
        }

        public override bool IsInteractable () {		
            bool interactable = base.IsInteractable ();
            texts.ForEach (t => t.color = interactable ? (isPressed ? PressedColor : isHighlighted ? HighlightedColor : NormalColor) : DisabledColor);
            return interactable;
        }

        private void OnBecameInvisible() {
            isHighlighted = false;
            isPressed = false;
            OnPointerExit(null);
        }

    }

}