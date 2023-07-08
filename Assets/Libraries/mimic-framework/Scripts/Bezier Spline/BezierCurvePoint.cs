using System;
using UnityEngine;

using Algebra = Mimic.Math.Algebra;

namespace Mimic.BezierSpline {

    [Serializable]
    public class BezierCurvePoint : Algebra.Vector3 {
        
        [SerializeField]
        private BezierControlPointMode mode;
        public BezierControlPointMode Mode {
            get => mode;
            set => mode = value;
        }

        public BezierCurvePoint(BezierCurvePoint other) : this(other.x, other.y, other.z, other.mode) {
        }

        public BezierCurvePoint(Vector3 point, BezierControlPointMode mode = BezierControlPointMode.Free) 
            : this(point.x, point.y, point.z, mode) {
        }

        public BezierCurvePoint(float x, float y, float z, BezierControlPointMode mode = BezierControlPointMode.Free) 
            : base(x, y, z) {
            this.mode = mode;
        }
    }

}
