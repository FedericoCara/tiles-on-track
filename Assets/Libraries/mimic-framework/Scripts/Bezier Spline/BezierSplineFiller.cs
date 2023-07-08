using System.Collections.Generic;
using UnityEngine;

namespace Mimic.BezierSpline {

    public class BezierSplineFiller : MonoBehaviour {

        [SerializeField]
        private BezierSpline spline;

        [SerializeField]
        private int frequency;

        [SerializeField]
        private bool lookForward;

        [SerializeField]
        public List<Transform> items;

        private void Start() {
            if (frequency <= 0 || items.IsEmpty()) {
                return;
            }
            int instantiatedItemsCount = frequency * items.Count;
            float stepSize;
            if (spline.IsLooped || instantiatedItemsCount == 1) {
                stepSize = 1f / instantiatedItemsCount;
            } else {                
                stepSize = 1f / (instantiatedItemsCount - 1);
            }
            for (int p = 0, f = 0; f < frequency; f++) {
                for (int i = 0; i < items.Count; i++, p++) {
                    Transform item = Instantiate<Transform>(items[i], transform);
                    Vector3 position = spline.GetWorldPointByProportionalLength(p * stepSize);
                    item.transform.position = position;
                    if (lookForward) {
                        item.transform.LookAt(position + spline.GetDirection(p * stepSize));
                    }
                }
            }
        }
    }

}