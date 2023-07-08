using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Mimic.UI {

    public class JoystickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
        
        [SerializeField]
        protected Image image;

        [SerializeField]
        protected Sprite normalState;

        [SerializeField]
        protected Sprite pressedState;

        [SerializeField]
        protected float sensibility = 0.3f;

        protected bool isPressed = false;

        protected float input = 0;

        public float Input {
            get => input;
        }

        protected virtual void Update () { 
            input = Mathf.Lerp(input, isPressed ? 1 : 0, sensibility);
        }
        
        public void OnPointerDown(PointerEventData data) {
            image.sprite = pressedState;
            isPressed = true;
        }

        public void OnPointerUp(PointerEventData data) {
            image.sprite = normalState;
            isPressed = false;
        }

        public void OnPointerExit(PointerEventData eventData) {
            OnPointerUp(eventData);
        }
    }
}
