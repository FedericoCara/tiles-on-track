using System;
using System.Collections.Generic;
using UnityEngine;

using Mimic;

namespace Mimic.BezierSpline {

    public class BezierSpline : MonoBehaviour, ISerializationCallbackReceiver {

        [SerializeField]
        private List<BezierCurve> curves = new List<BezierCurve>();
        public List<BezierCurve> Curves => curves;

        [SerializeField]
        private bool isLooped;
        public bool IsLooped {
            get => isLooped;
            set {
                if (isLooped != value) {
                    isLooped = value;
                    if(isLooped) {
                        StitchEnds();
                    } else {
                        BezierCurve lastCurve = curves.GetLastElement<BezierCurve>();
                        lastCurve.FinalPoint = new BezierCurvePoint(lastCurve.FinalPoint);
                    }
                }
            }
        }

        public int CurveCount => curves.Count;

        public Vector3 InitialPoint => transform.TransformPoint(curves[0].InitialPoint);

        private void OnDrawGizmos() {
            //curves.ForEach(curve => curve.DrawGizmos(transform));
        }

        public void OnBeforeSerialize() {
            Stich();
        }

        public void OnAfterDeserialize() {
            Stich();
        }

        private void Stich() {
            int curvesCount = CurveCount;
            if(curvesCount > 0) {
                int i;
                for(i = 0; i < curvesCount - 1; i++) {
                    curves[i].FinalPoint = curves[i + 1].InitialPoint;
                }
                if(isLooped) {
                    StitchEnds(false);
                }
            }
        }

        public Vector3 GetWorldPoint(float t) {
            for (int i = 0; i < CurveCount; i++) {
                if(t <= curves[i].Length) {
                    return transform.TransformPoint(curves[i].GetPoint(t / curves[i].Length));
                } else {
                    t -= curves[i].Length;
                }
            }
            return transform.TransformPoint(curves.GetLastElement<BezierCurve>().FinalControlPoint);
        }

        public Vector3 GetWorldPointByLength(float length) {
            float totalSplineLength = CalculateSplineLength();
            return GetWorldPointByProportionalLength(length / totalSplineLength, totalSplineLength);
        }

        public Vector3 GetWorldPointByProportionalLength(float normalizedLength) {
            return GetWorldPointByProportionalLength(normalizedLength, CalculateSplineLength());
        }

        private Vector3 GetWorldPointByProportionalLength(float normalizedLength, float totalSplineLength) {
            Vector3 point = curves.GetLastElement<BezierCurve>().FinalPoint;
            if (normalizedLength < 1f) {          
                float tRelativeToCurveRemaining = normalizedLength * totalSplineLength;                                
                for (int i = 0; i < CurveCount; i++) {
                    if(tRelativeToCurveRemaining <= curves[i].Length) {
                        point = curves[i].GetPoint(tRelativeToCurveRemaining / curves[i].Length);
                        break;
                    } else {
                        tRelativeToCurveRemaining -= curves[i].Length;
                    }
                }
            }
            return transform.TransformPoint(point);
        }

        public Vector3 GetVelocity(float t) {            
            for (int i = 0; i < CurveCount; i++) {
                if(t <= curves[i].Length) {
                    return transform.TransformPoint(curves[i].GetFirstDerivative(t)) - transform.position;
                } else {
                    t -= curves[i].Length;
                }
            }
            BezierCurve lastCurve = curves.GetLastElement<BezierCurve>();
            return transform.TransformPoint(lastCurve.GetFirstDerivative(t)) - transform.position;
        }

        public Vector3 GetDirection(float t) {
            return GetVelocity(t).normalized;
        }

        public Vector3 GetDirectionByLength(float length) {
            float totalSplineLength = CalculateSplineLength();
            return GetDirectionByProportionalLength(length / totalSplineLength, totalSplineLength);
        }
        
        public Vector3 GetDirectionByProportionalLength(float normalizedLength) {
            return GetDirectionByProportionalLength(normalizedLength, CalculateSplineLength());
        }

        private Vector3 GetDirectionByProportionalLength(float normalizedLength, float totalSplineLength) {
            if (normalizedLength < 1f) {          
                float tRelativeToCurveRemaining = normalizedLength * totalSplineLength;                                
                for (int i = 0; i < CurveCount; i++) {
                    if(tRelativeToCurveRemaining <= curves[i].Length) {
                        Vector3 firstDerivative = curves[i].GetFirstDerivativeByNormalizedLength(tRelativeToCurveRemaining / curves[i].Length);
                        return transform.TransformPoint(firstDerivative.normalized) - transform.position; //transform.TransformPoint(firstDerivative) ;//- transform.position;
                    } else {
                        tRelativeToCurveRemaining -= curves[i].Length;
                    }
                }
            }
            return GetDirection(1);
        }

        public float CalculateSplineLength() {
            float totalSplineLength = 0;
            curves.ForEach(curve => totalSplineLength += curve.Length);
            return totalSplineLength;
        }

        public void AddCurve(Vector3 newCurveOffset, int curveIndex) {
            //TODO add a new curve in the middle using curveIndex
            BezierCurve lastCurve = curves.GetLastElement<BezierCurve>();
            BezierCurvePoint initialPoint = lastCurve.FinalPoint;
            Vector3 initialControlPoint = initialPoint + newCurveOffset;
            Vector3 finalControlPoint = initialControlPoint + newCurveOffset;
            BezierCurvePoint finalPoint;
            if(isLooped) {
                finalPoint = initialPoint;
                initialPoint = new BezierCurvePoint(finalPoint);
                lastCurve.FinalPoint = initialPoint;
            } else {
                finalPoint = new BezierCurvePoint(finalControlPoint + newCurveOffset, lastCurve.FinalPoint.Mode);
            }
            curves.Add(new BezierCurve(initialPoint, initialControlPoint, finalPoint, finalControlPoint));
            EnforceMode(CurveCount - 2, false);
            if(isLooped){
                EnforceMode(0, true);
            }
        }

        public void RemoveCurve(int curveIndex) {
            if(curveIndex < 0 || curveIndex == curves.Count - 1) {
                curves.RemoveLastElement<BezierCurve>();
                if(IsLooped) {
                    StitchEnds();
                }
            } else {
                //TODO remove point selected (recalculating adjacent curves) instead of the next curve
                curves.RemoveAt(curveIndex);
                if(curveIndex == 0) {
                    curves[0].InitialPoint = curves.GetLastElement<BezierCurve>().FinalPoint;
                    EnforceMode(CurveCount - 1, true);
                } else {
                    curves[curveIndex].InitialPoint = curves[curveIndex - 1].FinalPoint;
                    EnforceMode(curveIndex - 1, true);
                }
            }
        }

        private void StitchEnds(bool enforceMode = true) {
            curves.GetLastElement<BezierCurve>().FinalPoint = curves[0].InitialPoint;
            if(enforceMode) {
                EnforceMode(0, true);
            }
        }

        public void Reset() {
            isLooped = false;
            curves.Clear();
            BezierCurvePoint initialPoint = new BezierCurvePoint(1f, 1f, 0f, BezierControlPointMode.Free);
            BezierCurvePoint finalPoint = new BezierCurvePoint(4f, 4f, 0f, BezierControlPointMode.Free);
            curves.Add(new BezierCurve(initialPoint, new Vector3(2f, 2f, 0f), finalPoint, new Vector3(3f, 3f, 0f)));
        }

        public void SetInitialPoint(int curveIndex, Vector3 point) {
            curves[curveIndex].InitialPoint.Set(point);
            EnforceMode(curveIndex, true);
        }

        public void SetInitialControlPoint(int curveIndex, Vector3 point) {
            curves[curveIndex].InitialControlPoint = point;
            EnforceMode(curveIndex, true);
        }

        public void SetFinalPoint(int curveIndex, Vector3 point) {
            curves[curveIndex].FinalPoint.Set(point);
            EnforceMode(curveIndex, false);
        }

        public void SetFinalControlPoint(int curveIndex, Vector3 point) {
            curves[curveIndex].FinalControlPoint = point;
            EnforceMode(curveIndex, false);
        }

        public void SetPointMode(int curveIndex, bool isInitial, BezierControlPointMode mode) {
            BezierCurve curve = curves[curveIndex];
            if(isInitial) {
                curve.InitialPoint.Mode = mode;
            } else {                
                curve.FinalPoint.Mode = mode;
            }
            EnforceMode(curveIndex, isInitial);
        }

        private void EnforceMode(int index, bool isInitialControlPointAsReference) {
            BezierCurve curve = curves[index];
            BezierCurve enforcedCurve;

            BezierControlPointMode mode = isInitialControlPointAsReference ? curve.InitialPoint.Mode : curve.FinalPoint.Mode; 
            if(mode == BezierControlPointMode.Free) {
                return;
            }

            if(CurveCount == 1) {
                if(isLooped) {
                    enforcedCurve = curve;
                } else {
                    return;
                }
            } else if(isInitialControlPointAsReference) {
                if(index == 0) {
                    if(isLooped) {
                        enforcedCurve = curves.GetLastElement<BezierCurve>();
                    } else {
                        return;
                    }
                } else {
                    enforcedCurve = curves[index - 1];
                } 
            } else {
                if(index == CurveCount - 1) {
                    if(isLooped) {
                        enforcedCurve = curves[0];
                    } else {
                        return;
                    }
                } else {
                    enforcedCurve = curves[index + 1];
                } 
            }

            BezierCurvePoint middle;
            Vector3 enforcedTangent;
            if(isInitialControlPointAsReference) {         
                //Change the position of control point of the final point in the previous curve
                middle = curve.InitialPoint;
                enforcedTangent = middle - curve.InitialControlPoint;
                if (mode == BezierControlPointMode.Aligned) {
                    enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, enforcedCurve.FinalControlPoint);
                }
                enforcedCurve.FinalControlPoint = middle + enforcedTangent;
            } else {
                //Change the position of control point of the initial point in the next curve
                middle = curve.FinalPoint;
                enforcedTangent = middle - curve.FinalControlPoint;
                if (mode == BezierControlPointMode.Aligned) {
                    enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, enforcedCurve.InitialControlPoint);
                }
                enforcedCurve.InitialControlPoint = middle + enforcedTangent;
            }
        }
        
    }

}