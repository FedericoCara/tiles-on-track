using System.Collections.Generic;
using UnityEngine;

namespace Mimic.Math {
    [System.Serializable]
    public class Coefficient : Fraction {

        private float angle;
        public float Angle {
            get { return angle; }
        }

        private Color color;
        public Color Color {
            get { return color; }
            set { color = value; }
        }

        public Coefficient(int x, int y) : this(x, y, Color.red) { }

        public Coefficient(int x, int y, Color color) : base(x, y) {
            if (value < 0) {
                angle = 180 + Mathf.Atan(value) * Mathf.Rad2Deg;
            } else {
                angle = Mathf.Atan(value) * Mathf.Rad2Deg;
            }
            this.color = color;
        }
    }
}