using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mimic.UI {
    
    public class ReelSpinner<T> : MonoBehaviour where T : ReelSpinnerElement {
        
        private List<T> elements = new List<T>();
        public List<T> Elements {
            get { return elements; }
        }
        public virtual List<int> Values {
            set {
                elements.Clear();
                snapper.Content.DestroyAllChildren(true);
                T newElement;
                int currentValue;
                for (int i = 0; i < value.Count; i++) {
                    currentValue = value[i];
                    newElement = Instantiate<T>(itemPrefab, snapper.Content);
                    newElement.name = itemPrefab.name + " - "+ currentValue;
                    newElement.Value = currentValue;
                    elements.Add(newElement);
                }
                needSnapping = true;
                //TODO check if this creates issues in other projects...
                LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
            }
        }
        public virtual Vector2Int Range {
            set{
                elements.Clear();
                snapper.Content.DestroyAllChildren(true);
                T newElement;
                for (int i = value.x; i <= value.y; i++) {
                    newElement = Instantiate<T>(itemPrefab, snapper.Content);
                    newElement.Value = i;
                    elements.Add(newElement);
                }
                needSnapping = true;
                //TODO check if this creates issues in other projects...
                LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
            }
        }

        [SerializeField]
        private VerticalScrollSnap snapper;
        public VerticalScrollSnap Snapper { get { return snapper; } }

        public RectTransform Content {
            get{ return snapper.Content; }
        }

        [SerializeField]
        private T itemPrefab;

        public int SelectedValue {
            get{ return selectedItem!=null ? selectedItem.Value : -1; }
            set{
                SelectedItem = snapper.ScrollRect.content.FindChild<T>(item => item.Value == value);                
                snapper.SnapTo(selectedItem.GetComponent<RectTransform>());

                //This code below should be improved, as not it won't always have an infinite scroll 
                infiniteScroll.AutomaticScroll();
                verticalLayoutGroup.enabled = false;
            }
        }

        private T selectedItem;
        private T SelectedItem {
            set{
                if(value != null && value != selectedItem) {
                    if(selectedItem != null) {
                        selectedItem.Selected = false;
                    }                
                    selectedItem = value;
                    selectedItem.Selected = true;
                    OnSelectedItemChanged();
                }
            }
        }

        private bool needSnapping = false;
        private InfiniteScroll infiniteScroll;
        private VerticalLayoutGroup verticalLayoutGroup;


        [SerializeField]
        private bool snapOnStart = true;

        protected virtual void Awake() {
            infiniteScroll = GetComponent<InfiniteScroll>();
            verticalLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>();
        }

        protected virtual void Start() {
            snapper.ScrollRect.onValueChanged.AddListener(position => OnScroll());
            if (snapOnStart) {
                snapper.Snap();
                OnScroll();
            }
        }

        protected virtual void Update() {
            if(needSnapping) {
                snapper.Snap();
                needSnapping = false;
            }
        }

        protected virtual void OnScroll() {
            RectTransform closestChildToCenter = snapper.GetClosestChildToCenter();
            if(closestChildToCenter != null) {
                SelectedItem = closestChildToCenter.GetComponent<T>();                
            }
        }

        protected virtual void OnSelectedItemChanged() {

        }
    }
}
