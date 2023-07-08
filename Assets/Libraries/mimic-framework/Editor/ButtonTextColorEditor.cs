using UnityEditor;
using UnityEngine;

using Mimic.UI;

namespace Mimic.Editor {

    [CustomEditor(typeof(ButtonTextColor))]
    [CanEditMultipleObjects]
    public class ButtonTextColorEditor : UnityEditor.UI.ButtonEditor {

        ButtonTextColor targetButton;

        protected override void OnEnable() {
            base.OnEnable();
            targetButton = (ButtonTextColor) target;
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();

            targetButton.NormalColor = EditorGUILayout.ColorField("Normal text color", targetButton.NormalColor);
            targetButton.HighlightedColor = EditorGUILayout.ColorField("Highlighted text color", targetButton.HighlightedColor);
            targetButton.PressedColor = EditorGUILayout.ColorField("Pressed text color", targetButton.PressedColor);
            targetButton.DisabledColor = EditorGUILayout.ColorField("Disabled text color", targetButton.DisabledColor);

            EditorGUILayout.Space();
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck()) {
                foreach (ButtonTextColor obj in targets) {
                    obj.NormalColor = targetButton.NormalColor;
                    obj.HighlightedColor = targetButton.HighlightedColor;
                    obj.PressedColor = targetButton.PressedColor;
                    obj.DisabledColor = targetButton.DisabledColor;
                    EditorUtility.SetDirty(obj);
                }
            }
        }

    }

}
