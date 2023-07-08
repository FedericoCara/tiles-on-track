using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mimic;

namespace Mimic.UI {

    [RequireComponent(typeof(Text))]
    public class AutoTypeText : MonoBehaviour {

        [SerializeField]
        protected float timeIntervalBetweenLetters;
        public float TimeIntervalBetweenLetters {
            set => timeIntervalBetweenLetters = value;
        }

        private bool autoType = true;
        public bool AutoType {
            get => autoType; 
            set { 
                if(autoType && !value && !finished) {
                    index = text.Length;
                    TextComponent.text = text;
                    OnFinish();
                }
                autoType = value;
            }
        }

        protected Text textComponent;
        public virtual Text TextComponent {
            get {
                if (textComponent == null) {
                    textComponent = GetComponent<Text>();
                }
                return textComponent;
            }
        }

        protected string text;
        protected Stack<string> openTags = new Stack<string>();

        protected Action onFinishedTypingAction;
        public Action OnFinishedTypingAction {
            set => onFinishedTypingAction = value;
        }

        protected int index = 0;
        protected bool finished = true;
        public bool HasFinished => finished;

        public virtual string Text {
            set {
                text = value;
                if(autoType && timeIntervalBetweenLetters > 0) {
                    index = 0;
                    if(!string.IsNullOrEmpty(value) && value.GetRichTextLength() > 0) {
                        TextComponent.text = "";
                        if(finished) {
                            finished = false;
                            StartCoroutine(TypeText());
                        } 
                    } else if(finished) {
                        TextComponent.text = value;
                        FireOnFinishedTypeActions();
                    }
                } else {
                    TextComponent.text = value;
                    index = value.Length; 
                    OnFinish();
                }                  
            }
        }

        protected virtual void Awake() {
            textComponent = GetComponent<Text>();
        }

        protected virtual IEnumerator TypeText () {
            while(index <= text.GetRichTextLength()) {
                TextComponent.text = text.RichTextSubString(index++);
                // if (sound) {
                //     audio.PlayOneShot (sound);
                // }
                if(index < text.GetRichTextLength()) {
                    yield return new WaitForSecondsRealtime (timeIntervalBetweenLetters);
                }
            }
            OnFinish();
        }

        protected virtual void FireOnFinishedTypeActions() {             
            onFinishedTypingAction?.Invoke();
        }

        protected virtual void OnFinish() {
            finished = true;
            FireOnFinishedTypeActions();
        }

        public virtual void Finish() {
            TextComponent.text = text.RichTextSubString(text.GetRichTextLength());
            OnFinish();
        }

    }

}