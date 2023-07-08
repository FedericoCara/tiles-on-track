using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DG.Tweening;

namespace Mimic.UI.Hover {

    public class HoverPanel : MonoBehaviour {

        [SerializeField]
        private RectTransform canvasRect;

        [SerializeField]
        private float onMouseOverStayTime = 0.5f;
        public float OnMouseOverStayTime {
            get{ return onMouseOverStayTime; }
        }

        [SerializeField]
        private float fadeInTweenDuration = 0.5f;
        [SerializeField]
        private float fadeOutTweenDuration = 0.1f;

        [SerializeField]
        protected Text title;
        public virtual string Title {
            get{ return title.text; }
            set{ title.text = value; }
        }

        [SerializeField]
        protected Text description;
        public virtual string Description {
            get{ return description.text; } 
            set{ description.text = value; }
        }

        [SerializeField]
        private Image topLeftArrow;
        [SerializeField]
        private Image topRightArrow;
        [SerializeField]
        private Image bottomLeftArrow;

        [SerializeField]
        private Image bottomRightArrow;

        [SerializeField]
        private bool showArrows = false;

        [SerializeField]
        protected Image contentBackround;

        private HoverElement element;
        public HoverElement Element { 
            get{ return element; }
        }

        private RectTransform rectTransform;

        private bool cancelHide = false;
        private float remainingTime = 0;

        public virtual Color Color {
            set{ 
                contentBackround.color = value; 
                title.color = value;
            }
        }

        protected virtual void Awake(){
            rectTransform = GetComponent<RectTransform> ();
        }

        protected virtual void Update(){
            if (element != null && remainingTime > 0) {
                remainingTime -= Time.unscaledDeltaTime;
                if (remainingTime <= 0) {
                    remainingTime = 0;
                    DelayHide ();
                }
            }
        }

        private void UpdatePosition() {
            #if UNITY_EDITOR || UNITY_WEBGL
            rectTransform.position = new Vector3(element.transform.position.x, element.transform.position.y, 0);
            #else
            float x = (Input.mousePosition.x / (float)Screen.width-rectTransform.anchorMin.x) * canvasRect.sizeDelta.x;
            float y = (Input.mousePosition.y / (float)Screen.height-rectTransform.anchorMin.y) * canvasRect.sizeDelta.y;
            rectTransform.anchoredPosition3D = new Vector3(x, y, 0);
            #endif

            //float x2 = (rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x) / canvasRect.sizeDelta.x;
            //float y2 = (rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y) / canvasRect.sizeDelta.y;
            //Debug.Log ("x2: "+x2+",y2: "+y2+", sizeDelta: "+canvasRect.sizeDelta);
            //Vector2 pivot = new Vector2(x2 < 0.5f ? 0 : 1, y2 < -0.5f ? 0 : 1);

            float pivotX = rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x < canvasRect.sizeDelta.x ? 0 : 1;
            float pivotY = rectTransform.anchoredPosition.y + rectTransform.sizeDelta.y < canvasRect.sizeDelta.y ? 0 : 1;

            if(topLeftArrow != null) {
                topLeftArrow.gameObject.SetActive(showArrows && pivotX == 0 && pivotY == 1);
            }
            if(topRightArrow != null) {
                topRightArrow.gameObject.SetActive(showArrows && pivotX == 1 && pivotY == 1);
            }
            if(bottomLeftArrow != null) {
                bottomLeftArrow.gameObject.SetActive(showArrows && pivotX == 1 && pivotY == 0);
            }
            if(bottomRightArrow != null) {
                bottomRightArrow.gameObject.SetActive(showArrows && pivotX == 0 && pivotY == 0);
            }

            rectTransform.pivot = new Vector2(pivotX, pivotY);
        }

        // //Esta bazofia la hago para corregir un error de update de los layout vertical group
        // protected virtual void LateUpdate(){
        //     string postaText = description.text;
        //     description.text = description.text+"d";
        //     description.text = postaText;
        // }

        public void Show(HoverElement element){
            if(this.element == element) {
                return;
            }

            ResetElement();            
            this.element = element;

            Title = element.Title;
            Description = element.Text;
            Color = element.Color;
                
            gameObject.SetActive (true);
            UpdatePosition();

            //rectTransform.pivot = new Vector2 (0f, 1f);
            Fade (1f, fadeInTweenDuration);
            cancelHide = true;
        }

        public void InmediateHide(){
            cancelHide = true;
            remainingTime = 0;
            gameObject.SetActive (false);
            ResetElement();
        }

        private void ResetElement() {
            if(this.element != null) {
                this.element.Reset();
            }
        }

        public void Hide() {		
            #if UNITY_EDITOR
            remainingTime = 1f;
            #elif UNITY_WEBGL
            remainingTime = 1f;
            #else
            DelayHide();
            #endif
            cancelHide = false;
        }

        public void DelayHide(){
            if (cancelHide) {
                return;
            }

            Fade (0f, fadeOutTweenDuration).OnComplete(() => {
                gameObject.SetActive(false);
                foreach (Graphic graphics in GetComponentsInChildren<Graphic>()) {
                    Color color = graphics.color;
                    color.a = 1f;
                    graphics.color = color;
                }
            });

            ResetElement();
        }

        private Sequence Fade(float target, float duration){
            Sequence sequence = DOTween.Sequence ();

            foreach (Graphic graphics in GetComponentsInChildren<Graphic>()) {
                sequence.Join (graphics.DOFade (target, duration));
            }

            sequence.SetUpdate (true);

            return sequence;
        }

    }
     
}
