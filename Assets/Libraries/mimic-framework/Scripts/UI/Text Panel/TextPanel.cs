using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Mimic;

namespace Mimic.UI {

    public class TextPanel : MonoBehaviour {

        [SerializeField]
        protected Text titleTxt;
        public virtual string Title {
            set => titleTxt.text = value;
        }
        
        [SerializeField]
        protected AutoTypeText bodyTxt;
        public virtual AutoTypeText BodyTxt => bodyTxt; 

        [SerializeField]
        protected bool isAutoTypeEnabled = true;

        public virtual bool IsAutoTypeEnabled {
            set {
                isAutoTypeEnabled = value;
                bodyTxt.AutoType = value;                
            }
        }
        
        public virtual float TimeIntervalBetweenLetters {
            set => bodyTxt.TimeIntervalBetweenLetters = value;
        }

        public virtual string Body {
            set {                
                if(speedUpTypingMask != null) {
                    speedUpTypingMask.gameObject.SetActive(true);
                }
                bodyTxt.Text = value;  
            }
        }

        [SerializeField]
        protected GameObject speedUpTypingMask;

        [SerializeField]
        protected GameObject graphicContainer;

        protected TextPanelGraphic graphic;

        public virtual TextPanelGraphic Graphic {
            get => graphic;
            set {
                if(graphic != value) {
                    if(value == null ) {
                        if(graphicContainer != null) {
                            graphicContainer.gameObject.SetActive(false);
                            Utils.DestroyChildren(graphicContainer.transform);
                        }
                    } else {
                        Utils.ReplaceChildrenWithGO(graphicContainer.transform, value.gameObject);
                        graphicContainer.gameObject.SetActive(true);
                    }
                    graphic = value;
                }
            }
        }

        public virtual bool Active {
            get => gameObject.activeSelf;
            protected set {         
                bodyTxt.AutoType = value && isAutoTypeEnabled;
                gameObject.SetActive(value);  
            }
        }

        protected virtual void Awake() {
            bodyTxt.OnFinishedTypingAction = OnFinishedTypingAction;
        }

        protected virtual void Set(string title, string body, bool showGraphic) {
            if(titleTxt != null) {
                titleTxt.text = title;
            }
            Body = body;
            if(graphicContainer != null) {
                graphicContainer.SetActive(showGraphic);
            }
        }

        public virtual void Set(string title, string content) {
            Set(title, content, false);
        }
        
        public virtual void Set(string title, string content, TextPanelGraphic graphic) {
            Set(title, content, true);
            Graphic = graphic;
        }

        protected virtual void OnFinishedTypingAction() {
            bodyTxt.AutoType = isAutoTypeEnabled;
            if(speedUpTypingMask != null) {
                speedUpTypingMask.gameObject.SetActive(false);
            }
        }
        
        public virtual void SpeedUpTyping() {
            bodyTxt.AutoType = false;
            if(speedUpTypingMask != null) {
                speedUpTypingMask.gameObject.SetActive(false);
            }
        }

    }

}