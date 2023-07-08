using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mimic.UI {
    public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

        [SerializeField]
        private bool interactable = true;
        public bool Interactable { get { return interactable; } set { interactable = value; } }

        [SerializeField]
        private Camera targetCamera;
        public virtual Camera TargetCamera { get { return targetCamera; } set { targetCamera = value; } }

        [SerializeField]
        private Canvas targetCanvas;
        public Canvas TargetCanvas { get { return targetCanvas; } set { targetCanvas = value; } }

        [SerializeField]
        private bool worldSpace = true;
        [SerializeField]
        private bool fixedZ = false;
        [SerializeField]
        private bool useLocalPosition = true;

        private Vector3 startingOffset;
        Transform startParent;

        protected RectTransform RectTransform { get { return GetComponent<RectTransform>(); } }

        void Start() {
            if (targetCamera == null) {
                targetCamera = Camera.main;
            }
            if (targetCanvas == null) {
                targetCanvas = GetComponentInParent<Canvas>();
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData) {
            if (!interactable) {
                return;
            }

            if (worldSpace) {
                Vector3 screenToWorldPoint = targetCamera.ScreenToWorldPoint(eventData.position);
                if(fixedZ)
                    screenToWorldPoint = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, transform.position.z);
                if (useLocalPosition) {
                    startingOffset = transform.localPosition - screenToWorldPoint;
                } else {
                    startingOffset = transform.position - screenToWorldPoint;
                }
            } else {
                if (useLocalPosition) {
                    startingOffset = RectTransform.localPosition - Input.mousePosition;
                } else {
                    startingOffset = RectTransform.position - Input.mousePosition;
                }
                startingOffset.z = 0;
            }
            startParent = transform.parent;
        }

        public virtual void OnDrag(PointerEventData eventData) {
            if (!interactable) {
                return;
            }

            if (worldSpace) {
                Vector3 screenToWorldPoint = targetCamera.ScreenToWorldPoint(eventData.position);
                if(fixedZ)
                    screenToWorldPoint = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, transform.position.z);
                if (useLocalPosition) {
                    this.transform.localPosition = screenToWorldPoint + (Vector3)startingOffset;
                } else {
                    this.transform.position = screenToWorldPoint + (Vector3)startingOffset;
                }
                //Debug.Log("Dragging startingOffset-"+startingOffset+", mousePosition-"+eventData.position+", screenToWorldPoint-"+screenToWorldPoint, gameObject);
                List<RaycastResult> resultAppendList = new List<RaycastResult>();
                targetCamera.GetComponent<PhysicsRaycaster>().Raycast(eventData, resultAppendList);
                OnDraggedOver(eventData, resultAppendList);
            } else {
                if (useLocalPosition) {
                    Vector2 mouseInWorldSpace;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    transform.parent as RectTransform ,
                    Input.mousePosition, TargetCanvas.worldCamera,
                    out mouseInWorldSpace);
                    RectTransform.localPosition = mouseInWorldSpace;
                } else {
                    RectTransform.position = Input.mousePosition + (Vector3)startingOffset;
                }
                List<RaycastResult> resultAppendList = new List<RaycastResult>();
                TargetCanvas.GetComponent<GraphicRaycaster>().Raycast(eventData, resultAppendList);
                OnDraggedOver(eventData, resultAppendList);
            }
        }

        public virtual void OnDraggedOver(PointerEventData eventData, List<RaycastResult> objectsDraggedOver) {

        }

        public virtual void OnEndDrag(PointerEventData eventData) {

        }
    }
}