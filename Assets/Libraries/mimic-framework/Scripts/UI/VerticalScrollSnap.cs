using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mimic.UI {
    [RequireComponent(typeof(ScrollRect))]
    public class VerticalScrollSnap : MonoBehaviour {

        [SerializeField]
        private float endOfInertiaThreshold = 10;

        private ScrollRect scrollRect;
        public ScrollRect ScrollRect {
            get { return scrollRect; }
        }
        public RectTransform Content {
            get{ return scrollRect.content; }
        }

        private float lastVelocity;
        private float lastContentPositionY;

        private bool needSnapping = false;
        private bool onInertia = false;

        private List<Action<Transform>> listeners = new List<Action<Transform>>();  

        private RectTransform lastSnappedTransform;
        public RectTransform LastSnappedTransform {
            get{ return lastSnappedTransform; }
        } 

        private void Awake() {
            scrollRect = GetComponent<ScrollRect>();
            Snap();
            //scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            lastContentPositionY = scrollRect.content.GetComponent<RectTransform>().anchoredPosition.y;
        }

        private void Update() {
            if (onInertia && Mathf.Abs(scrollRect.velocity.y) <= endOfInertiaThreshold &&
                scrollRect.velocity.y != lastVelocity) {
                onInertia = false;
                scrollRect.velocity = new Vector2(scrollRect.velocity.x,0);
                Snap();
            }
            lastVelocity = scrollRect.velocity.y;
        }

        public void RemoveChild(int index) {
            Destroy(scrollRect.content.GetChild(index).gameObject);
            Snap();
        }

        public T AddChild<T>(T prefab) where T : MonoBehaviour {
            T newChild = Instantiate<T>(prefab, scrollRect.content);
            Snap();
            return newChild;
        }

        public void Snap() {
            SnapTo(GetClosestChildToCenter());            
        }

        public void OnEndDrag() {
            if (scrollRect.velocity.y == 0) {
                Snap();
            } else {
                onInertia = true;
            }
        }
        
        public void SnapTo(RectTransform child) {
            RectTransform otherChild;
            float offset = GetOffset(child);
            for (int i = 0; i < scrollRect.content.childCount; i++) {
                otherChild = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
                Vector2 position = otherChild.anchoredPosition;
                position.y += offset;
                otherChild.anchoredPosition = position;
            }
            lastSnappedTransform = child;
            FireOnSnapEvent();
        }

        public RectTransform GetClosestChildToCenter() {   
            float minDistance = float.MaxValue;
            float minOffset = 0;
            float distanceToCenter;
            float offset;
            RectTransform closestChildToCenter = null;
            RectTransform child;
            for (int i = 0; i < scrollRect.content.childCount; i++) {
                child = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
                offset = GetOffset(child);
                distanceToCenter = Mathf.Abs(offset);
                if(distanceToCenter < minDistance) {
                    minOffset = offset;
                    minDistance = distanceToCenter;
                    closestChildToCenter = child;
                }
            }            
            
            // print("CenterY = "+centerY+", cententOffsetY = "+contentOffsetY);
            // print("minDistance = "+minDistance+", minOffset = "+minOffset);
            // print("Closest Child = "+closestChildToCenter.GetComponent<Text>().text);
            // print("Position = "+closestChildToCenter.position);
            // print("Anchored Position = "+closestChildToCenter.GetComponent<RectTransform>().anchoredPosition);
            return closestChildToCenter;
        }

        public RectTransform GetBestSnappingTarget() {
            float minDistance = float.MaxValue;
            float minOffset = 0;
            float distanceToCenter;
            float offset;
            RectTransform closestChildToCenter = null;
            RectTransform child;
            for (int i = 0; i < scrollRect.content.childCount; i++) {
                child = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
                offset = GetOffset(child);
                distanceToCenter = Mathf.Abs(offset);
                if (distanceToCenter < minDistance) {
                    minOffset = offset;
                    minDistance = distanceToCenter;
                    closestChildToCenter = child;
                }
            }

            // print("CenterY = "+centerY+", cententOffsetY = "+contentOffsetY);
            // print("minDistance = "+minDistance+", minOffset = "+minOffset);
            // print("Closest Child = "+closestChildToCenter.GetComponent<Text>().text);
            // print("Position = "+closestChildToCenter.position);
            // print("Anchored Position = "+closestChildToCenter.GetComponent<RectTransform>().anchoredPosition);
            return closestChildToCenter;
        }

        private float GetOffset(RectTransform child) {
            float viewPortHalfSize = scrollRect.GetComponent<RectTransform>().sizeDelta.y / 2;
            float contentOffsetY = scrollRect.content.GetComponent<RectTransform>().anchoredPosition.y;
            return - viewPortHalfSize - contentOffsetY - child.GetComponent<RectTransform>().anchoredPosition.y;
        }

        public void AddListener(Action<Transform> listener) {
            listeners.Add(listener);
        }

        private void FireOnSnapEvent(){
            listeners.ForEach(listener => listener(lastSnappedTransform));
        }
    }

}
