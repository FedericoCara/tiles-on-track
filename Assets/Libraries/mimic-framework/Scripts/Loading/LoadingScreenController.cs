using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Mimic {

    public class LoadingScreenController : MonoBehaviour {

        [SerializeField]
        private Image loadingImg;

        [SerializeField]
        private float minLoadingTime = 1;
        public float MinLoadingTime => minLoadingTime;
        
        private bool isLoading = false;

        protected virtual void ShowLoadingScreen() {
            if (isLoading) {
                return;
            }
            AsyncSceneLoader sceneLoader = AsyncSceneLoader.Instance;
            sceneLoader.onStartLoadingEvent += OnStartLoading;
            sceneLoader.onLoadingEvent += OnLoading;
            sceneLoader.onFinishLoadingEvent += OnFinishLoading;
            gameObject.SetActive(true);
        }

        public void LoadScene(string sceneName) {
            ShowLoadingScreen();
            AsyncSceneLoader.Instance.LoadScene(sceneName, minLoadingTime);
        }

        protected virtual void OnLoading(float progress) {
            if(loadingImg != null) {
                loadingImg.fillAmount = progress;
            }
        }

        protected virtual void OnStartLoading() {
        }
        
        protected virtual void OnFinishLoading() {
        }

    }
}