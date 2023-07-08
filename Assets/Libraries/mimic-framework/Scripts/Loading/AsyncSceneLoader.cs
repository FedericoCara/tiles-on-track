using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mimic {

    public class AsyncSceneLoader : AutoInstantiableSingleton<AsyncSceneLoader> {

        public override bool DestroyOnLoad => false;

        private float waitingMinLoadingTime = 0;
        private float waitingMinLoadingTimeLeft = 0;
        
        private AsyncOperation async;

        private bool allowSceneActivationWhenComplete = true;
        public bool AllowSceneActivationWhenComplete {
            set {                
                if(allowSceneActivationWhenComplete != value) {
                    allowSceneActivationWhenComplete = value;
                    if(allowSceneActivationWhenComplete && readyToActivateLoadedScene) {
                        ActivateLoadedScene();
                    }
                }
            }
        }

        private bool readyToActivateLoadedScene = false;

        public delegate void OnSceneStartedLoading();
        public event OnSceneStartedLoading onStartLoadingEvent;
        public delegate void OnSceneFinishedLoading();
        public event OnSceneFinishedLoading onFinishLoadingEvent;
        public delegate void OnSceneLoading(float progress);
        public event OnSceneLoading onLoadingEvent;

        protected void Update() {
            if (async == null) {
                return;
            }
            
            waitingMinLoadingTimeLeft = Mathf.Clamp(waitingMinLoadingTimeLeft-Time.deltaTime, 0, waitingMinLoadingTime);

            #if UNITY_EDITOR || UNITY_WEBGL
            float progress = (waitingMinLoadingTime - waitingMinLoadingTimeLeft) / waitingMinLoadingTime * 0.9f;
            #else
            float progress = async.progress;
            #endif

            if(progress < 0.9f) {
                //scene not loaded yet
                onLoadingEvent?.Invoke(progress / 0.9f);
            } else if(waitingMinLoadingTimeLeft <= 0) {
                onFinishLoadingEvent?.Invoke();
                readyToActivateLoadedScene = true;
                if(allowSceneActivationWhenComplete) {
                    ActivateLoadedScene();
                }
            }
        }

        public void LoadScene(string sceneName, float minWaitingTime = 0) {
            if(async != null) {
                Debug.LogWarning("Scene already being loaded");
                return;
            }
            waitingMinLoadingTimeLeft = waitingMinLoadingTime = minWaitingTime;
            onStartLoadingEvent?.Invoke();
            async = SceneManager.LoadSceneAsync (sceneName); // load game	
            async.allowSceneActivation = false;
        }

        private void ActivateLoadedScene() {
            async.allowSceneActivation = true;
            ClearPreviousLoad();
        }

        private void ClearPreviousLoad(){
            async = null;
            onStartLoadingEvent = null;
            onFinishLoadingEvent = null;
            onLoadingEvent = null;
            waitingMinLoadingTimeLeft = 0;
            waitingMinLoadingTime = 0;
            readyToActivateLoadedScene = false;
            allowSceneActivationWhenComplete = true;
        }
    }
}