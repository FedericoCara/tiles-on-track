using System.Collections;

using UnityEditor;
using UnityEngine;

using Mimic.UI;

namespace Mimic.Editor {

    public class ListView {

        private static readonly Color defaultBackground = GUI.backgroundColor;
        private static readonly Color selectedBackground = Color.gray;
        
        private GUIStyle itemStyle = new GUIStyle(GUI.skin.button);  //make a new GUIStyle
        
        private int selectedIndex = -1;
        public int SelectedIndex {
            set {
                if(value < list.Count) {
                    selectedIndex = value;
                } else {
                    selectedIndex = -1;
                }
            }
        }

        private IList list;
        public IList List {
            set {
                list = value;
                if(selectedIndex >= list.Count) {
                    selectedIndex = -1;
                }
            }
        }

        public ListView(IList list) {
            this.list = list;
            itemStyle.alignment = TextAnchor.MiddleLeft; //align text to the left
            itemStyle.active.background = itemStyle.normal.background;  //gets rid of button click background style.
            itemStyle.margin = new RectOffset(0, 0, 0, 0); //removes the space between items (previously there was a small gap between GUI which made it harder to select a desired item)
        }

        public void OnGUI() {
        
            for (int i = 0; i < list.Count; i++) {
                GUI.backgroundColor = selectedIndex == i ? selectedBackground : Color.clear;
                
                //show a button using the new GUIStyle
                if (GUILayout.Button(list[i].ToString(), itemStyle)) {
                    //Switch Off
                    selectedIndex = selectedIndex == i ? -1 : i;
                }
            
                GUI.backgroundColor = defaultBackground; //this is to avoid affecting other GUIs outside of the list
            }

        }

    }

}