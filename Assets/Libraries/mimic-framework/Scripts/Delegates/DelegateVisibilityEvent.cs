using UnityEngine;
using System.Collections;

namespace Mimic.Animations
{
    public class DelegateVisibilityEvent : MonoBehaviour {

        public GameObject target;

        private void OnBecameVisible() {
            target.SendMessage(nameof(OnBecameVisible), SendMessageOptions.DontRequireReceiver);
        }
        
        private void OnBecameInvisible() {
            target.SendMessage(nameof(OnBecameInvisible), SendMessageOptions.DontRequireReceiver);
        }
    }
}