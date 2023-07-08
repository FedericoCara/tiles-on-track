using UnityEngine;


namespace Mimic {

    public class Splash : MonoBehaviour {
        
        [SerializeField]
        protected float timer = 1;

        [SerializeField]
        protected string nextScene;

        protected float timeRemaining;

        protected bool finished = false;

        protected virtual void Start() {
            timeRemaining = timer;
        }

        protected virtual void Update() {
            if(finished) {
                return;
            }

            timeRemaining -= Time.deltaTime;
            if(timeRemaining < 0) {
                AsyncSceneLoader.Instance.LoadScene(nextScene, 0);
                finished = true;
            }
        }

    }

}
