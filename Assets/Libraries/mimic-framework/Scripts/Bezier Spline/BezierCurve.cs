using System;
using System.Collections.Generic;

using UnityEngine;

namespace Mimic.BezierSpline {

    [Serializable]
    public class BezierCurve {
        
        public const int Steps = 500;

        [SerializeField]
        private BezierCurvePoint initialPoint;
        public BezierCurvePoint InitialPoint {
            get => initialPoint;
            set {
                if(initialPoint != value) {
                    initialPoint = value;
                    CalculateLength();
                }
            }
        }

        [SerializeField]
        private Vector3 initialControlPoint;
        public Vector3 InitialControlPoint {
            get => initialControlPoint;
            set {
                if(initialControlPoint != value) {
                    initialControlPoint = value;
                    CalculateLength();
                }
            }
        }

        [SerializeField]
        private BezierCurvePoint finalPoint;
        public BezierCurvePoint FinalPoint {
            get => finalPoint;
            set {
                if(finalPoint != value) {
                    finalPoint = value;
                    CalculateLength();
                }
            }
        }

        [SerializeField]
        private Vector3 finalControlPoint;
        public Vector3 FinalControlPoint {
            get => finalControlPoint;
            set {
                if(finalControlPoint != value) {
                    finalControlPoint = value;
                    CalculateLength();
                }
            }
        }

        [SerializeField]
        private Color color = Color.black;

        private List<CurveIntegralPoint> integratedPoints;
        public List<CurveIntegralPoint> IntegralPoints => integratedPoints;

        private float length;
        public float Length {
            get {
                if(length == 0) {
                    CalculateLength();
                }
                return length;
            }
        }

        public bool IsLinear =>    initialPoint.Mode == BezierControlPointMode.Linear 
                                ||   finalPoint.Mode == BezierControlPointMode.Linear;

        public BezierCurve(BezierCurvePoint initialPoint, Vector3 initialControlPoint, BezierCurvePoint finalPoint, Vector3 finalControlPoint) {
            this.initialPoint = initialPoint;
            this.initialControlPoint = initialControlPoint;
            this.finalPoint = finalPoint;
            this.finalControlPoint = finalControlPoint;
        }

        private void CalculateLength() {
            if (IsLinear) {
                length = (finalPoint - initialPoint).Magnitude;
            } else {
                length = 0;
                float distanceToPreviousPoint;
                Vector3 point;
                Vector3 prevPoint = initialPoint;
                if(integratedPoints == null) {
                    integratedPoints = new List<CurveIntegralPoint>(Steps + 1);
                } else {
                    integratedPoints.Clear();
                }

                float stepsAsFloat = (float) Steps;
                for (int i = 1; i <= Steps; i++) {
                    point = Bezier.GetPoint(initialPoint, InitialControlPoint, FinalControlPoint, finalPoint, i / stepsAsFloat);
                    distanceToPreviousPoint = (point - prevPoint).magnitude;
                    length += distanceToPreviousPoint;
                    integratedPoints.Add(new CurveIntegralPoint(i / stepsAsFloat, prevPoint, distanceToPreviousPoint));
                    prevPoint = point;
                }
                integratedPoints.Add(new CurveIntegralPoint(1, finalPoint, 0));
            }
        }

        public Vector3 GetPoint(float normalizedLength) {
            if (normalizedLength >= 1) {
                return finalPoint;
            } else if (normalizedLength <= 0) {
                return initialPoint;
            } else if (IsLinear) {
                return (finalPoint - initialPoint) * normalizedLength;
            } else {  

                // This can be replaced by a binary search where each point stores the accumulated length
                // rather than the distance to the next point

                if(integratedPoints == null) {
                    CalculateLength();
                }
                int i = 0;
                while(normalizedLength > 0 && i <= Steps) {
                    normalizedLength -= integratedPoints[i++].DistanceToNextPoint / length;
                }
                return integratedPoints[i - 1].Point + (integratedPoints[i].Point - integratedPoints[i - 1].Point) * -normalizedLength;
            }
        }

        public Vector3 GetFirstDerivative(float t) {
            return Bezier.GetFirstDerivative(initialPoint, InitialControlPoint, FinalControlPoint, finalPoint, t);
        }

        public Vector3 GetFirstDerivativeByNormalizedLength(float normalizedLength) {
            
            if (normalizedLength >= 1) {
                return finalPoint - integratedPoints[integratedPoints.Count - 2].Point;
            } else if (normalizedLength <= 0) {
                return integratedPoints[1].Point - initialPoint;
            } else if (IsLinear) {
                return (finalPoint - initialPoint) * normalizedLength;
            } else {  

                // This can be replaced by a binary search where each point stores the accumulated length
                // rather than the distance to the next point

                if(integratedPoints == null) {
                    CalculateLength();
                }
                int i = 0;
                while(normalizedLength > 0 && i <= Steps) {
                    normalizedLength -= integratedPoints[i++].DistanceToNextPoint / length;
                }
                return integratedPoints[i].Point - integratedPoints[i - 1].Point;
            }

        }

        public bool OverlapAllPoints(Collider2D collider) {
            return integratedPoints.TrueForAll(point => collider.OverlapPoint(point.Point));
        }

        public void DrawGizmos(Transform parent) {  
            if (IsLinear) {
                Gizmos.DrawLine(parent.TransformPoint(initialPoint), parent.TransformPoint(finalPoint));
            } else {
                int i = 0;
                while(i < 19) {
                    Gizmos.DrawLine(GetPoint(i / 20f), GetPoint(++i / 20f));
                }
            }
        }
    }

}
