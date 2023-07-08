using UnityEngine;
using UnityEngine.EventSystems;

namespace Mimic.UI.Hover {

    public class HoverElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [SerializeField]
        private Color color = Color.white;
        public Color Color {
            get{ return color; }
        }

        [SerializeField]
        private string title;
        public string Title {
            get{ return title; }
            set{ title = value; }
        }

        [SerializeField, TextArea]
        private string text;
        public string Text {
            get{ return text; }
            set{ text = value; }
        }

        [SerializeField]
        private HoverPanel hoverPnl;
        public HoverPanel HoverPnl {
            set{ hoverPnl = value; }
        }

        private float stayTime;

        private bool mouseOver = false;
        private bool active;

        public void Update() {
            if (mouseOver && (!hoverPnl.gameObject.activeSelf || hoverPnl.Element != this)) {
                stayTime += Time.unscaledDeltaTime;
                if (!active && stayTime > hoverPnl.OnMouseOverStayTime) {
                    hoverPnl.Show (this);
                    active = true;
                }
            }
        }

        public void Reset() {
            stayTime = 0;
            active = false;
            mouseOver = false;
        }

        public void OnPointerEnter(PointerEventData eventData) {
    //		print ("Enter: "+name);
    //		if (hoverPnl.gameObject.activeSelf && hoverPnl.Element != this)
    //			hoverPnl.Element.OnPointerExit(null);
            if (!mouseOver) {
                mouseOver = true;
                stayTime = 0;
            } 
        }

        public void OnPointerExit(PointerEventData eventData){
    //		print ("Exit: "+name);
            if (hoverPnl.gameObject.activeSelf) {
                hoverPnl.Hide ();
            }
            if (mouseOver) {
                stayTime = 0;
                mouseOver = false;
                active = false;
            }
        }

    }

}
