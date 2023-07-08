using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mimic.UI {

    [RequireComponent(typeof(Toggle))]
    public abstract class ListItem<T> : MonoBehaviour where T : class {
        protected Toggle toggle;

        [SerializeField]
        protected Color oddColor = Color.white, evenColor = Color.white;

        [SerializeField]
        protected Image background;

        private bool isOdd;
        public bool IsOdd {
            get { return isOdd; }
            set {
                isOdd = value;
                if (background != null) {
                    background.color = value ? oddColor : evenColor;
                }
            }
        }

        protected T data;
        public T Data {
            get { return data; }
            set {
                data = value;
                name = data.ToString();
                Refresh();
            }
        }

        public bool IsSelected {
            get { return toggle != null ? toggle.isOn : false; }
            set {
                toggle.isOn = value;
                Refresh();
            }
        }

        private float timeSinceLastSelection = 0;
        public float TimeSinceLastSelection {
            get { return timeSinceLastSelection; }
        }

        public bool interactable {
            set { if (toggle != null) toggle.interactable = value; }
        }

        public Vector2 anchoredPosition {
            get { return GetComponent<RectTransform>().anchoredPosition; }
        }

        public Vector2 size {
            get { return GetComponent<RectTransform>().sizeDelta; }
        }

        protected virtual void Awake() {
            toggle = GetComponent<Toggle>();
        }

        protected virtual void Update() {
        }

        protected abstract void Refresh();

        public void SetToggleAction(UnityAction<bool> callback) {
            toggle.onValueChanged.AddListener(SetTimeSinceLastSelection);
            toggle.onValueChanged.AddListener(callback);
        }

        private void SetTimeSinceLastSelection(bool isOn) {
            timeSinceLastSelection = Time.realtimeSinceStartup;
            Refresh();
        }
    }

}