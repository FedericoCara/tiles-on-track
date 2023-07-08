using System;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace Mimic.Effects {

    public class Fade : MonoBehaviour {

        [SerializeField]
        private float totalTime;
        public float TotalTime {
            get => totalTime;
        }

        [SerializeField]
        private float startPeriod;

        [SerializeField]
        private float endPeriod;

        [SerializeField]
        private int steps = 4;

        public void StartFading(Action onFadeEnded) {
            StartFading(transform.FindChildren<Renderer>(renderer => renderer.gameObject.activeInHierarchy), onFadeEnded);            
        }

        public void StartFading(List<Renderer> renderers, Action onFadeEnded) {
            if(renderers.IsEmpty()) {
                onFadeEnded?.Invoke();
                return;
            }
            Sequence sequence = DOTween.Sequence();
            if(steps < 1) {
                renderers.ForEach(renderer => {
                    if(renderer.material.HasProperty("_Color")) {
                        sequence.Join(renderer.material.DOFade(0, totalTime));
                    }
                });
            } else {
                float timeRamaining = totalTime - endPeriod - startPeriod;
                //float slope = (endPeriod - startPeriod) / numberOfIntervals;
                //float weight = Mathf.Pow(endPeriod / startPeriod, 1f / steps);
                float weight = Math.Utils.SolvePolynomialEquation(timeRamaining / startPeriod, steps - 1, 0.0001f);                
            
                renderers.ForEach(renderer => {
                    if(renderer.material.HasProperty("_Color")) {
                        sequence.Join(CreateFadeSequence(renderer, steps, weight));
                    }
                });
            }
            //renderers.ForEach(renderer => sequence.Join(GetComponentInChildren<Renderer>().material.DOFade(0, totalTime)));
            if(onFadeEnded != null) {
                sequence.OnComplete(() => onFadeEnded());
            }
        }

        private Sequence CreateFadeSequence(Renderer renderer, int numberOfIntervals, float weight) {
            float total = 0;
            float duration;  
            Sequence sequence = DOTween.Sequence();        
            for(int i=0; i<numberOfIntervals; i++) {
                total += duration = startPeriod * Mathf.Pow(weight, i);    
                sequence.Append(renderer.material.DOFade(0, duration / 2));
                sequence.Append(renderer.material.DOFade(1, duration / 2));
            }            
            sequence.Append(renderer.material.DOFade(0, endPeriod / 2));
            //sequence.Append(renderer.material.DOFade(1, endPeriod / 2));
            return sequence;
        }

        public void Reset() {
            transform.FindChildren<Renderer>(renderer => renderer.gameObject.activeInHierarchy).ForEach(renderer => {
                if(renderer.material.HasProperty("_Color")) {
                    Color color = renderer.material.GetColor("_Color");
                    color.a = 1;
                    renderer.material.SetColor("_Color", color);
                }
            });
        }

    }

}