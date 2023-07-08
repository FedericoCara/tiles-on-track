using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Mimic.UI {

    public class SwitchTextColor : MonoBehaviour {
        [SerializeField]
        private Text text;

        [SerializeField]
        private Color normalColor;

        [SerializeField]
        private Color selectedColor;

        public void OnToggleSelected(bool selected) {
            if (selected) {
                text.color = selectedColor;
            } else {
                text.color = normalColor;
            }
        }
    }

}