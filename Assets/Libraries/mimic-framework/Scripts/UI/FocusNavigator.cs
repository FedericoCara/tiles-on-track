using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI {
    
    public class FocusNavigator : MonoBehaviour {

        [SerializeField] private int autoSelectedIndex = 0;

        protected List<Selectable> selectables;

        protected Selectable currentSelectable;
        public virtual Selectable CurrentSelectable {
            get { return currentSelectable; }
            set {
                if (currentSelectable is Dropdown) {
                    ((Dropdown)currentSelectable).Hide(); ;
                }
                currentSelectable = value;
                currentSelectable.Select();

                if (currentSelectable is Dropdown) {
                    ((Dropdown)currentSelectable).Show(); ;
                }
            }
        }

        private int currentSelectableIndex;
        public int CurrentSelectableIndex {
            get { return currentSelectableIndex; }
            set {
                if (value > currentSelectableIndex)
                    currentSelectableIndex = FixSelectableIndex(value);
                else if (value < currentSelectableIndex) {
                    currentSelectableIndex = FixSelectableIndex(value);
                }
                CurrentSelectable = selectables[currentSelectableIndex];
            }
        }

        private int FixSelectableIndex(int index) {
            if (index >= selectables.Count)
                return 0;
            if (index < 0)
                return selectables.Count - 1;
            return index;
        }

        private void OnEnable() {
            selectables = new List<Selectable>(GetComponentsInChildren<Selectable>());
            for (int i = 0; i < selectables.Count; ++i) {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnDown = navigation.selectOnRight = selectables[FixSelectableIndex(i + 1)];
                navigation.selectOnUp = navigation.selectOnLeft = selectables[FixSelectableIndex(i - 1)];
                selectables[i].navigation = navigation;
            }
                StartCoroutine(SelectSelectable(autoSelectedIndex));
        }

        private IEnumerator SelectSelectable(int index) {
            yield return new WaitForEndOfFrame();
            CurrentSelectableIndex = index;
        }

        private void Update() {
            if (InputAdapter.GetKeyDown(KeyCode.Tab)) {
                if (InputAdapter.GetKey(KeyCode.LeftShift)) {
                    CurrentSelectableIndex--;
                } else {
                    CurrentSelectableIndex++;
                }
            }
        }
    }
}