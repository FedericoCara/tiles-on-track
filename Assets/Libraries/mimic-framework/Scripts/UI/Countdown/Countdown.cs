using System;

using UnityEngine;

using DG.Tweening;

namespace Mimic.UI {

    public abstract class Countdown<T> : MonoBehaviour where T : MonoBehaviour {

        [SerializeField]
        protected T target;
        public T Target {
            get => target;
        }

        [SerializeField]
        protected float punchScale = 1.2f;

        [SerializeField]
        protected float punchScaleDuration = 0.5f;

        [SerializeField]
        protected bool isTimeScaleIndependent = true;
        public bool IsTimeScaleIndependent {
            set {
                isTimeScaleIndependent = value;
                punchScaleTween.SetUpdate(value);
                countdownSequence?.SetUpdate(value);
            }
        }   


        private Tweener punchScaleTween;

        private Sequence countdownSequence;

        private float punchScaleIntervals;

        protected int count;

        private Action onFinish;
        public Action OnFinish {
            set => onFinish = value;
        }

        private Action<int> onIntervalStart;
        public Action<int> OnIntervalStart {
            set{ onIntervalStart = value; }
        }

        private Action onIntervalFinish;
        public Action OnIntervalFinish {
            set{ onIntervalFinish = value; }
        }

        protected void Awake() {
            punchScaleIntervals = Mathf.Max(0, 1 - punchScaleDuration);
            punchScaleTween = target.transform.DOPunchScale(new Vector3(punchScale, punchScale, 1), punchScaleDuration)
                                            .SetUpdate(true)
                                            .SetAutoKill(false)
                                            .OnPlay(OnTweenIntervalStarted)
                                            .OnRewind(OnTweenIntervalStarted)
                                            .Pause();            
        }
        
        public void StartCountdown(int start) {
            count = start;
            if(countdownSequence == null) {
                countdownSequence = DOTween.Sequence().SetUpdate(isTimeScaleIndependent);
                countdownSequence.Append(punchScaleTween)
                        .AppendInterval(punchScaleIntervals)
                        .SetAutoKill(false);
                if(onFinish != null) {
                    countdownSequence.OnComplete(() => onFinish());
                }
            } else {
                countdownSequence.Restart();
            }
            countdownSequence.SetLoops(count);
        }

        protected abstract void SetNextValue();

        private void OnTweenIntervalStarted() {
            SetNextValue();
            onIntervalStart?.Invoke(count);
        }

        private void OnTweenIntervalFinished() {
            onIntervalFinish?.Invoke();
        }

    }

}