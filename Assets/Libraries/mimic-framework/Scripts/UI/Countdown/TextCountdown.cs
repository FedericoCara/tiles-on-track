using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI {

    public class TextCountdown : Countdown<Text> {

        [SerializeField]
        private string zeroText = "0";
        public string ZeroText {
            set => zeroText = value;
        }        

        protected override void SetNextValue() {
            target.text = --count == 0 ? zeroText : count.ToString();
        }
    }

}