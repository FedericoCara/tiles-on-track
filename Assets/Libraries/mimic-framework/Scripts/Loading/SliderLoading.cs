using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic
{
    public class SliderLoading : LoadingScreenController
    {
        [SerializeField]
        private Slider sheepSlider;

        protected override void OnLoading(float progress) {
            base.OnLoading(progress);
            sheepSlider.value = progress;
        }
    }
}