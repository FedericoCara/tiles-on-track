
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI {

    public class Console : Singleton<Console> {

        [SerializeField]
        private Text text;

        [SerializeField]
        private Scrollbar scrollBar;

        public void writeln(string newLine) {
            Debug.Log(newLine);
            string newText = text.text + "\n" + newLine;
            //Debug.Log("newline length: "+newText.Length);
            if (newText.Length > 5000) {
                newText = newText.Substring(1000);
            }
            text.text = newText;
            Invoke(nameof(ResetScrollBarPosition), 0.05f);
        }

        public void Clear() {
            text.text = "";
            scrollBar.size = 1;
            scrollBar.value = 1;
        }

        public void ResetScrollBarPosition() {
            scrollBar.value = 0;
        }
    }
}