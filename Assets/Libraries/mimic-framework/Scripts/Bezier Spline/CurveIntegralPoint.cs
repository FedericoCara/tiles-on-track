using UnityEngine;

namespace Mimic.BezierSpline {

    public class CurveIntegralPoint {

        private Vector3 point;
        public Vector3 Point => point;

        private float t;
        private float T => t;

        private float distanceToNextPoint;
        public float DistanceToNextPoint => distanceToNextPoint;

        public CurveIntegralPoint(float t, Vector3 point, float distanceToNextPoint) {
            this.t = t;
            this.point = point;
            this.distanceToNextPoint = distanceToNextPoint;
        }
        
    }
    
}