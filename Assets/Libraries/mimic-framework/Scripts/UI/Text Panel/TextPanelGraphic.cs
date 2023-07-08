using System;
using UnityEngine;
namespace Mimic.UI {

    public class TextPanelGraphic : MonoBehaviour {

        [SerializeField]
        protected Animator animator;

        protected virtual void Awake() {
            if(animator!=null)
                animator = GetComponent<Animator>();
        }
        
        public virtual void Stop() {
            if(animator != null) {
                animator.ResetTrigger("Start");
                animator.SetTrigger("Init");
            }
        }

        public virtual void Play() {
            if(animator != null) {
                animator.ResetTrigger("Init");
                animator.SetTrigger("Start");
            }
        }

    }

}