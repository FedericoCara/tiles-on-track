using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Mimic {

    /// <summary>
    /// Shows information related to the transform component.
    /// It used to be called DebugPosition
    /// </summary>    
    public class TransformDebugger : MonoBehaviour {

        [SerializeField]
        private Vector3 position;

        [SerializeField]
        private Vector3 localPosition;

        [SerializeField]
        private Vector3 mainCameraScreenPosition;

        [SerializeField]
        private Vector3 mainCameraViewportPosition;

        [SerializeField]
        private List<Vector3> screenPositions = new List<Vector3>();

        [SerializeField]
        private List<Vector3> viewportPositions = new List<Vector3>();      

        [SerializeField]
        private bool printToConsole = false;

        [SerializeField, ConditionalField(nameof(printToConsole))]
        private float printPeriod = 1;

        [SerializeField, ConditionalField(nameof(printToConsole))]
        private bool printScreenPosition = true;

        [SerializeField, ConditionalField(nameof(printToConsole))]
        private bool printViewportPosition = true;

        [SerializeField, ConditionalField(nameof(printToConsole))]
        private bool printAnchoredPosition = true;

        [SerializeField]
        private bool useMainCam = true;

        [SerializeField]
        private List<Camera> targetCams;

        private float timeAlive;

        private RectTransform rectTransform;

        private StringBuilder sb = new StringBuilder();

        protected void Awake() {
            rectTransform = GetComponent<RectTransform>();
        }

        protected void Update() {
            position = transform.position;
            localPosition = transform.localPosition;
            mainCameraScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            mainCameraViewportPosition = Camera.main.WorldToViewportPoint(transform.position);

            screenPositions.Clear();            
            viewportPositions.Clear();
            if(targetCams != null) {
                targetCams.ForEach(camera => {
                    if(camera != null) {
                        screenPositions.Add(camera.WorldToScreenPoint(position));
                        viewportPositions.Add(camera.WorldToViewportPoint(position));
                    }
                });
            }

            if(printToConsole) {
                timeAlive += Time.deltaTime;
                if (timeAlive > printPeriod) {
                    timeAlive = 0;
                    PrintToConsole();
                }
            }
        }

        private void PrintToConsole() {
            sb.Clear();
            sb.Append("World position: ");
            sb.AppendLine(position.ToString());
            if (printAnchoredPosition && rectTransform != null) {
                sb.Append("Anchor Position: ");
                sb.AppendLine(rectTransform.anchoredPosition.ToString());
            }
            if (printScreenPosition) {
                if (useMainCam) {
                    sb.Append("Screen position: ");
                    sb.AppendLine(mainCameraScreenPosition.ToString());
                } else {
                    sb.Append("Screen positions: ");
                    AddPositionsToStringBuilder(screenPositions);
                }
            }
            if (printViewportPosition) {
                if (useMainCam) {
                    sb.Append("Viewport position: ");
                    sb.AppendLine(mainCameraViewportPosition.ToString());
                } else {
                    sb.Append("Viewport positions: ");
                    AddPositionsToStringBuilder(viewportPositions);
                }
            }
            Debug.Log(sb.ToString());
        }

        private void AddPositionsToStringBuilder(List<Vector3> positions) {
            if(positions.IsEmpty()) {
                return;
            }            

            sb.Append(positions[0].ToString());
            for (int i = 1; i < targetCams.Count; i++) {
                sb.Append(", ");
                sb.Append(positions[i].ToString());
            }
            sb.AppendLine();
        }
    }
}