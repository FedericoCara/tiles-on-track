using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI {

    public class MulticolorBar : MonoBehaviour {

        [SerializeField]
        private Image bar;

        [SerializeField]
        private List<Color> colors;

        public virtual float Value {
            get{ return bar.fillAmount; }
            set{ 
                bar.fillAmount = value;
                bar.color = colors[Mathf.RoundToInt((colors.Count - 1) * value)];
            }
        }

    }

}
