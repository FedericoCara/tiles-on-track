using UnityEngine;

namespace Mimic.BezierSpline {

    public class BezierSplineWalker : MonoBehaviour {

        [SerializeField]
        private BezierSpline spline;

        [SerializeField]
        private float duration;

        [SerializeField]
        private bool lookForward;       

        [SerializeField]
        private SplineWalkerMode mode;

        private float progress;
        private bool isGoingForward = true;

        private void Update() {
            if (isGoingForward) {
                progress += Time.deltaTime / duration;
                if (progress > 1f) {
                    if (mode == SplineWalkerMode.Once) {
                        progress = 1f;
                    } else if (mode == SplineWalkerMode.Loop) {
                        progress -= 1f;
                    } else {
                        progress = 2f - progress;
                        isGoingForward = false;
                    }
                }
            } else {
                progress -= Time.deltaTime / duration;
                if (progress < 0f) {
                    progress = -progress;
                    isGoingForward = true;
                }
            }

            Vector3 position = spline.GetWorldPoint(progress);
            transform.localPosition = position;
            if (lookForward) {
                transform.LookAt(position + spline.GetDirection(progress));
            }
        }
    }

}