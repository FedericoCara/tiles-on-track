using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mimic.Audio {

    public class MusicCrossfader : Crossfader {

        private Dictionary<AudioClip, float> audioClipTimes = new Dictionary<AudioClip, float>();

        protected override void Initialize() {
            base.Initialize();
            SceneManager.sceneLoaded += (scene, mode) => audioClipTimes.Clear();
        }
        
        public override void Crossfade(AudioClip clip, bool loop) {
            EnsureIntialized();
            AudioSource current = MainAudioSource;

            if(current.clip != null) {
                // Save current clip time
                if(audioClipTimes.ContainsKey(current.clip)) {
                    audioClipTimes[current.clip] = current.time;
                } else {
                    audioClipTimes.Add(current.clip, current.time);
                }
            }

            if(clip == null) {
                Stop(transitionDuration, () => current.clip = null);
            } else if(audioClipTimes.ContainsKey(clip)) {
                // Restore clip time
                Crossfade(clip, audioClipTimes[clip], loop);
            } else {
                base.Crossfade(clip, loop);
            }
        }

    }

}