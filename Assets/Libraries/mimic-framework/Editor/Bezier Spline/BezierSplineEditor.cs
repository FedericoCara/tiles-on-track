using System;
using UnityEditor;
using UnityEngine;

using Mimic.BezierSpline;

namespace Mimic.Editor {

    [CustomEditor(typeof(BezierSpline.BezierSpline))]
    public class BezierSplineEditor : UnityEditor.Editor {

        private const float directionScale = 0.05f;
        private const int stepsPerCurve = 10;
        private const float handleSize = 0.04f;
        private const float pickSize = 0.06f;

        private static Color[] modeColors = {
            Color.black,
            Color.yellow,
            Color.cyan,
            Color.white
        };

        private enum PointType {
            Initial,
            InitialControl,
            FinalControl,
            Final
        }

        private BezierSpline.BezierSpline spline;
        private Transform handleTransform;
        private Quaternion handleRotation;
        private Color handlesColor = Color.red;
        private Color lineColor = Color.black;
        private bool showDirection = false;
        private bool showIntegralPoints = false;

        private int selectedCurveIndex = -1;
        private PointType selectedPointType;

        private Vector3 newCurveOffset = new Vector3(1, 1, 0);
        
        public override void OnInspectorGUI() {
            spline = target as BezierSpline.BezierSpline;
            showIntegralPoints = EditorGUILayout.Toggle("Show Integral Points", showIntegralPoints);
            showDirection = EditorGUILayout.Toggle("Show directions", showDirection);
            handlesColor = EditorGUILayout.ColorField("Handles color", handlesColor);
            lineColor = EditorGUILayout.ColorField("Line color", lineColor);
            EditorGUI.BeginChangeCheck();
            bool isLooped = EditorGUILayout.Toggle("Is Looped", spline.IsLooped);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Toggle Is Looped");
                EditorUtility.SetDirty(spline);
                spline.IsLooped = isLooped;
            }
            bool isInitialPointInSplineSelected = selectedCurveIndex == 0 && selectedPointType == PointType.Initial;
            bool isFinalPointInSplineSelected = selectedCurveIndex == 0 && selectedPointType == PointType.Initial;
            if (selectedCurveIndex >= 0 && !isInitialPointInSplineSelected && !isFinalPointInSplineSelected) {
                DrawSelectedPointInspector();
            }
            newCurveOffset = EditorGUILayout.Vector3Field("New curve offset", newCurveOffset);
            if (GUILayout.Button("Add Curve")) {
                Undo.RecordObject(spline, "Add Curve");
                spline.AddCurve(newCurveOffset, -1);
                EditorUtility.SetDirty(spline);
            }
            if (spline.CurveCount > 1 && GUILayout.Button("Remove Curve")) {
                Undo.RecordObject(spline, "Remove Curve");
                spline.RemoveCurve(selectedCurveIndex);
                selectedCurveIndex = -1;
                EditorUtility.SetDirty(spline);
            }
        }

        private void DrawSelectedPointInspector() {
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Selected Point", GetSelectedPoint());
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                UpdateSelectedPoint(point);
            }

            EditorGUI.BeginChangeCheck();
            BezierControlPointMode mode = (BezierControlPointMode) EditorGUILayout.EnumPopup("Mode", GetSelectedPointMode());
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Change Point Mode");
                bool isInitial = selectedPointType == PointType.Initial || selectedPointType == PointType.InitialControl;
                spline.SetPointMode(selectedCurveIndex, isInitial, mode);
                EditorUtility.SetDirty(spline);
            }
        }

        private void OnSceneGUI() {
            spline = target as BezierSpline.BezierSpline;
            handleTransform = spline.transform;
            if(Tools.pivotRotation == PivotRotation.Local) {
                handleRotation =  handleTransform.rotation;
            } else {
                handleRotation = Quaternion.identity;
            }

            BezierCurve curve;
            for (int i = 0; i < spline.CurveCount; i++) {
                curve = spline.Curves[i];
                ShowPoint(i, PointType.Initial);
                ShowPoint(i, PointType.InitialControl);
                ShowPoint(i, PointType.FinalControl);
                ShowPoint(i, PointType.Final);  
                
                Handles.color = handlesColor;
                Vector3 initialPoint = handleTransform.TransformPoint(curve.InitialPoint);
                Vector3 initialControlPoint = handleTransform.TransformPoint(curve.InitialControlPoint);
                Vector3 finalControlPoint = handleTransform.TransformPoint(curve.FinalControlPoint);
                Vector3 finalPoint = handleTransform.TransformPoint(curve.FinalPoint);

                Handles.DrawLine(initialPoint, initialControlPoint);
                Handles.DrawLine(finalControlPoint, finalPoint);

                if (curve.IsLinear) {
                    Handles.color = lineColor;
                    Handles.DrawLine(initialPoint, finalControlPoint);
                } else {
                    Handles.DrawBezier(initialPoint, finalPoint, initialControlPoint, finalControlPoint, lineColor, null, 2f);
                }
            }
            if(showIntegralPoints) {
                ShowIntegralPoints();
            }
            if(showDirection) {
                ShowDirections();
            }
        }

        private Vector3 ShowPoint(int curveIndex, PointType pointType) {
            Vector3 point = handleTransform.TransformPoint(GetPoint(curveIndex, pointType));
            float size = HandleUtility.GetHandleSize(point);
            if (curveIndex == 0 && pointType == PointType.Initial) {
                size *= 2f;
            }

            Handles.color = GetHandleColor(curveIndex, pointType);
            if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
                selectedCurveIndex = curveIndex;
                selectedPointType = pointType;
                Repaint();
            }
            if (selectedCurveIndex == curveIndex && selectedPointType == pointType) {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    UpdateSelectedPoint(handleTransform.InverseTransformPoint(point));
                }
            }
            return point;
        }

        private void UpdateSelectedPoint(Vector3 point) {
            switch(selectedPointType) {
                case PointType.Initial: spline.SetInitialPoint(selectedCurveIndex, point); break;
                case PointType.InitialControl: spline.SetInitialControlPoint(selectedCurveIndex, point); break;
                case PointType.FinalControl: spline.SetFinalControlPoint(selectedCurveIndex, point); break;
                case PointType.Final: spline.SetFinalPoint(selectedCurveIndex, point); break;
                default: throw new ArgumentException("Point Type not supported");
            }
        }

        private Vector3 GetSelectedPoint() {
            return GetPoint(selectedCurveIndex, selectedPointType);
        }

        private Vector3 GetPoint(int curveIndex, PointType pointType) {
            BezierCurve curve = spline.Curves[curveIndex];
            switch(pointType) {
                case PointType.Initial: return curve.InitialPoint;
                case PointType.InitialControl: return curve.InitialControlPoint;
                case PointType.FinalControl: return curve.FinalControlPoint;
                case PointType.Final: return curve.FinalPoint;
                default: throw new ArgumentException("Point Type not supported");
            }
        }

        private Color GetHandleColor(int curveIndex, PointType pointType) {
            BezierCurve curve = spline.Curves[curveIndex];
            switch(pointType) {
                case PointType.Initial: return modeColors[(int) curve.InitialPoint.Mode];
                case PointType.Final: return modeColors[(int) curve.FinalPoint.Mode];
                default: return Color.magenta;
            }
        }

        private BezierControlPointMode GetSelectedPointMode() {
            return GetMode(selectedCurveIndex, selectedPointType);
        }

        private BezierControlPointMode GetMode(int curveIndex, PointType pointType) {
            BezierCurve curve = spline.Curves[curveIndex];
            switch(pointType) {
                case PointType.Initial:
                case PointType.InitialControl: return curve.InitialPoint.Mode;
                case PointType.FinalControl:
                case PointType.Final: return curve.FinalPoint.Mode;
                default: throw new ArgumentException("Point Type not supported");
            }
        }

        private void ShowIntegralPoints() {
            spline.Curves.ForEach(ShowIntegralPoints);
        }

        private void ShowIntegralPoints(BezierCurve curve) {
            if(curve.IntegralPoints == null) {
                return;
            }
            
            curve.IntegralPoints.ForEach(integralPoint => {
                Handles.DrawSolidDisc(spline.transform.TransformPoint(integralPoint.Point), Vector3.forward, 0.005f);
            });
        }

        private void ShowDirections() {
            Handles.color = Color.green;
            Vector3 point = spline.GetWorldPoint(0f);
            Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
            int steps = stepsPerCurve * spline.CurveCount;
            for (int i = 1; i <= steps; i++) {
                point = spline.GetWorldPointByProportionalLength(i / (float)steps);
                Handles.DrawLine(point, point + spline.GetDirectionByProportionalLength(i / (float)steps) * directionScale);
            }
        }

    }

}