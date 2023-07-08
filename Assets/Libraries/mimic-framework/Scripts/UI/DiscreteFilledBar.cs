using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI {

    public class DiscreteFilledBar : MonoBehaviour {

        [SerializeField]
        private List<Image> images;
        public List<Image> Images {
            get => images;
        }

        [SerializeField]
        private AnimationCurve valueMapping = AnimationCurve.Linear(0,0,1,1);

        public float Value {
            set {
                float intervalSize = 1f / images.Count;
                int filledIntervals = Mathf.CeilToInt(valueMapping.Evaluate(value) / intervalSize);
                int i = 0;
                images.ForEach(image => image.gameObject.SetActive((images.Count - i++) <= filledIntervals));
            }
        }

    }

}