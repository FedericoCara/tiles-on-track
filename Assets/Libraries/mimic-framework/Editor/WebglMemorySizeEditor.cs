using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Mimic.Editor {

    public class WebglMemorySizeEditor : EditorWindow {

        private int memSize;

        [MenuItem("Tools/Set Webgl Memory Size")]
        public static void ShowWindow() {
            GetWindow<WebglMemorySizeEditor>("Set Webgl Memory size", true).memSize = PlayerSettings.WebGL.memorySize;
        }

        private void OnGUI() {
            EditorGUILayout.Space();
            memSize = EditorGUILayout.IntField("Webgl Max Memory Size",memSize);
            PlayerSettings.WebGL.memorySize = memSize;
        }
    }
    
}

