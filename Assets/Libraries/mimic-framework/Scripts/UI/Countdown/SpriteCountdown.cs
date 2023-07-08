using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI {

    public class SpriteCountdown : Countdown<Image> {

        [SerializeField]
        private List<Sprite> sprites;

        protected override void SetNextValue() {
            target.sprite = sprites[--count];
        }
    }

}