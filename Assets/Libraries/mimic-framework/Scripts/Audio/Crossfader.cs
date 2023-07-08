using System;
using UnityEngine;

using DG.Tweening;
namespace Mimic.Audio {

    public class Crossfader : MonoBehaviour {

        [SerializeField]
        protected AudioSource audio1;
        public AudioSource AudioSource1 => audio1;

        [SerializeField]
        protected AudioSource audio2;
        public AudioSource AudioSource2 => audio2;

        [SerializeField]
        protected float transitionDuration = 3;

        private bool initialized = false;
        private float audio1StartingVolume;
        private float audio2StartingVolume;
        protected bool isGoingFrom1to2;
        protected bool isCrossfadingActive = true;

        public AudioSource MainAudioSource => isGoingFrom1to2 ? audio2 : audio1;

        protected virtual void Awake() {
            EnsureIntialized();
        }

        protected virtual void Update() {
            if(isGoingFrom1to2) {
                audio1.volume = Mathf.Clamp(audio1.volume - Time.deltaTime / transitionDuration, 0, audio1StartingVolume);
                audio2.volume = Mathf.Clamp(audio2.volume + Time.deltaTime / transitionDuration, 0, audio2StartingVolume);
            } else {
                audio1.volume = Mathf.Clamp(audio1.volume + Time.deltaTime / transitionDuration, 0, audio1StartingVolume);
                audio2.volume = Mathf.Clamp(audio2.volume - Time.deltaTime / transitionDuration, 0, audio2StartingVolume);
            }
        }

        protected void EnsureIntialized() {
            if(!initialized) {
                Initialize();
            }
        }

        protected virtual void Initialize() {
            initialized = true;
            audio1StartingVolume = audio1.volume;
            audio2StartingVolume = audio2.volume;
        }

        public virtual void PlayWithoutCrossfade(AudioClip clip, bool loop = true) {
            EnsureIntialized();
            isCrossfadingActive = false;
            if(isGoingFrom1to2) {
                audio1.Stop();
                audio2.clip = clip;
                audio2.loop = loop;
                audio2.Play();
                audio2.volume = audio2StartingVolume;
            } else {
                audio1.clip = clip;
                audio1.loop = loop;
                audio1.Play();
                audio1.volume = audio1StartingVolume;
                audio2.Stop();
            }
        }

        public virtual void Crossfade(AudioClip clip, bool loop = true) {
            Crossfade(clip, -1, loop);
        }

        public virtual void Crossfade(AudioClip clip, float timePosition, bool loop) {
            EnsureIntialized();
            isCrossfadingActive = true;
            if(audio1.volume > audio2.volume) {
                
                // Not sure why this is here.
                // if(!audio1.isPlaying) {
                //     audio1.Play();
                //     audio1.volume = audio1StartingVolume;
                // }

                audio2.clip = clip;
                audio2.time = (timePosition > 0 && timePosition < audio2.clip.length) ? timePosition : 0;
                audio2.loop = loop;
                audio2.Play();
                isGoingFrom1to2 = true;
            } else {

                // Not sure why this is here.
                // if(!audio2.isPlaying) {
                //     audio2.Play();
                //     audio2.volume = audio2StartingVolume;
                // }

                audio1.clip = clip;
                audio1.time = (timePosition > 0 && timePosition < audio1.clip.length) ? timePosition : 0;
                audio1.loop = loop;
                audio1.Play();
                isGoingFrom1to2 = false;
            }
        }

        public void Stop(float fadeOutDuration = 0, Action onComplete = null) {
            if(fadeOutDuration <= 0) {
                audio1.Stop();
                audio2.Stop();
            } else {
                isCrossfadingActive = false;

                Sequence sequence = DOTween.Sequence();
                sequence.Join(audio1.DOFade(0, fadeOutDuration));
                sequence.Join(audio2.DOFade(0, fadeOutDuration));
                sequence.OnComplete(() => {
                    audio1.Stop();
                    audio2.Stop();
                    onComplete?.Invoke();
                });
            }
        }

        public AudioSource GetAudioSourcePlayingClip(AudioClip audioClip) {
            if (audio1.clip == audioClip)
                return audio1;
            else if (audio2.clip == audioClip)
                return audio2;
            else
                return null;
        }

    }

}