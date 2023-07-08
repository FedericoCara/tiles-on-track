using UnityEngine;
using UnityEngine.UI;

namespace Mimic {

    public class FPSDisplay : Singleton<FPSDisplay> {

        [SerializeField]
        private Text fpsTxt;
        [SerializeField]
        private Text avgpsFpsTxt;
        [SerializeField]
        private Text avgpsFrameTimeTxt;

        //	[SerializeField]
        //	private float updateFPSPeriod = 0.5f;

        private float remainingTimeForAvgUpdate = 1f;
        private int frames = 0;
        private int totalFrames = 0;
        private float totalTIme = 0;
        private float timeSinceLastAVG = 0;

        void Awake() {
            instance = this;
            //gameObject.SetActive(false);
        }

        protected void Update() {
            UpdateFPSTxt(Time.unscaledDeltaTime);
            remainingTimeForAvgUpdate -= Time.unscaledDeltaTime;
            timeSinceLastAVG += Time.unscaledDeltaTime;
            totalTIme += Time.unscaledDeltaTime;
            frames++;
            totalFrames++;
            if (remainingTimeForAvgUpdate < 0) {
                avgpsFpsTxt.text = "AVG: " + frames.ToString();
                avgpsFrameTimeTxt.text = string.Format("AVG Ft.: {0:0.0} ms", totalTIme * 1000.0f / totalFrames);
                frames = 0;
                remainingTimeForAvgUpdate = 1f;
                timeSinceLastAVG = 0;
            }
        }

        private void UpdateFPSTxt(float deltaTime) {
            float fps = 1.0f / deltaTime;
            float msec = deltaTime * 1000.0f;
            fpsTxt.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        }
    }


}