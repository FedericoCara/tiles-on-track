using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Mimic;

namespace Mimic.UI {

    public class ListView<T, K> : MonoBehaviour where T : ListItem<K> where K : class {

        [SerializeField] protected T itemPrefab;
        [SerializeField] protected RectTransform content;
        [SerializeField] private bool interactable = true;
        [SerializeField] private int multipleSelectionMax = 2;
        [SerializeField] private bool poolItems = true;
        [SerializeField] private bool clearOnAwake = true;

        private List<T> itemsPool = new List<T>();

        public int MultipleSelectionMax {
            get { return multipleSelectionMax; }
            set { multipleSelectionMax = value; }
        }

        public bool Interactable {
            get { return interactable; }
            set { interactable = value; }
        }

        private ScrollRect scrollRect;

        protected List<T> items = new List<T>();
        public List<T> Items { get { return new List<T>(items); } }

        public List<K> Data {
            get {
                List<K> data = new List<K>(items.Count);
                items.ForEach(item => data.Add(item.Data));
                return data;
            }
            set {
                Clear();
                value.ForEach(d => Add(d));
            }
        }

        public int SelectedIndex {
            get {
                if (items != null) {
                    T selectedItem = items.Find(i => i.IsSelected);
                    return selectedItem != null ? items.IndexOf(selectedItem) : -1;
                } else {
                    Debug.Log("items NULL");
                    return -1;
                }
            }
        }

        public int[] SelectedIndexes {
            get {
                List<T> selectedItems = items.FindAll(i => i.IsSelected);

                int[] selectedIndexes = new int[selectedItems.Count];
                for (int i = 0; i < selectedIndexes.Length; i++) {
                    selectedIndexes[i] = items.IndexOf(selectedItems[i]);
                }
                return selectedIndexes;
            }
        }

        public T SelectedItem {
            get { return items.Find(i => i.IsSelected); }
        }

        public K SelectedItemData {
            get {
                T selectedItem = SelectedItem;
                return selectedItem == null ? null : selectedItem.Data;
            }
        }

        public K[] SelectedItemsData {
            get {
                List<K> list = new List<K>();
                items.FindAll(i => i.IsSelected).ForEach(i => list.Add(i.Data));
                return list.ToArray();
            }
        }

        public K LastSelectedItem {
            get {
                int[] selectedItems = SelectedIndexes;
                if (selectedItems.Length > 0) {
                    T item = null;
                    float oldTime = float.PositiveInfinity;
                    for (int i = 0; i < selectedItems.Length; i++) {

                        if (items[selectedItems[i]].TimeSinceLastSelection < oldTime) {
                            item = items[selectedItems[i]];
                            oldTime = item.TimeSinceLastSelection;
                        }
                    }
                    return item.Data;
                } else return null;
            }
        }

        private ToggleGroup group;
        public ToggleGroup Group {
            get {
                if (group == null)
                    group = GetComponent<ToggleGroup>();
                return group;
            }
        }

        public event Action OnListSelectionChange;
        public event Action<T> OnItemSelectionChange;

        protected virtual void Awake() {
            //Clear unwanted starting elements 
            if(clearOnAwake)
                Utils.DestroyChildren(content.transform);
        }

        protected virtual void Start() {
            scrollRect = GetComponent<ScrollRect>();
            CheckUpDown();
        }

        public void CheckUpDown() {
            float vertical = InputAdapter.GetAxis("Vertical");
            if (vertical > 0.05) {
                Up();
            } else if (vertical < -0.05) {
                Down();
            }
            Invoke(nameof(CheckUpDown), 0.1f);
        }

        [System.Obsolete("This should be replaced with the Data property. It will be removed in future versions.")]
        public virtual void Set(List<K> data) {
            Data = data;
        }

        public virtual void Clear() {
            if(poolItems) {
                items.ForEach(PutBackInPool);
            } else {
                Utils.DestroyChildren(content.transform);
            }
            items.Clear();
        }

        public void Remove(K itemData) {
            for(int i = items.Count-1;i>=0; --i) {
                if (items[i].Data == itemData) {
                    Remove(i);
                    return;
                }
            }            
        }

        public void Remove(int index) {
            if(poolItems) { 
                PutBackInPool(items[index]);      
            } else {
                Destroy(items[index].gameObject);
            }
            items.RemoveAt(index);
        }

        private void PutBackInPool(T item) {
            item.name = item.name + " (Pooled)";
            item.gameObject.SetActive(false);
            itemsPool.Add(item);    

        }

        public virtual T Add(K itemData) {
            return Add(itemData, itemPrefab);
        }

        public virtual T Add(K itemData, T prefab) {
            T item;
            if(!poolItems || itemsPool.Count == 0) {
                item = Instantiate<T>(prefab);
                item.transform.SetParent(content.transform, false);
                item.SetToggleAction(isOn => OnItemChange(item));
            } else {
                item = itemsPool[0];
                itemsPool.RemoveAt(0);
                item.transform.SetParent(content.parent);
                item.transform.SetParent(content.transform, false);
                item.gameObject.SetActive(true);
            }
            item.name = "item: " + itemData.GetType().Name;
            item.interactable = interactable;
            item.Data = itemData;
            item.IsOdd = items.Count % 2 == 0;
            items.Add(item);
            return item;
        }

        public void Set(int index, K itemData) {
            items[index].Data = itemData;
        }

        [System.Obsolete("This should be replaced with the SelectedIndexes property. It will be removed in future versions.")]
        public int[] GetSelectedIndexes() {
            return SelectedIndexes;
        }

        [System.Obsolete("This should be replaced with the SelectedItemsData property. It will be removed in future versions.")]
        public K[] GetSelectedItemsData() {
            return SelectedItemsData;
        }

        [System.Obsolete("This should be replaced with the SelectedItem property. It will be removed in future versions.")]
        public T GetSelectedItem() {
            return SelectedItem;
        }

        [System.Obsolete("This should be replaced with the SelectedItemData property. It will be removed in future versions.")]
        public K GetSelectedItemData() {
            return SelectedItemData;
        }

        [System.Obsolete("This should be replaced with the SelectedIndex property. It will be removed in future versions.")]
        public int GetSelectedIndex() {
            return SelectedIndex;
        }

        public void DeselectAll() {
            items.ForEach(i => i.IsSelected = false);
        }

        public void Deselect() {
            int[] selectedItems = SelectedIndexes;
            if (selectedItems.Length > 0) {
                T item = null;
                float oldTime = Time.realtimeSinceStartup;
                for (int i = 0; i < selectedItems.Length; i++) {

                    if (oldTime > items[selectedItems[i]].TimeSinceLastSelection) {
                        item = items[selectedItems[i]];
                        oldTime = item.TimeSinceLastSelection;
                    }
                }
                item.IsSelected = false;
            }
        }

        private void OnItemChange(T item) {
            while (item.IsSelected && SelectedIndexes.Length > MultipleSelectionMax) {
                Deselect();
            }
            FireSelectionChangedEvent(item);
        }

        public void Select(K itemData) {
            if (IsSelected(itemData))
                return;
            T item = items.Find(x => object.Equals(x.Data,itemData)); //To avoid ref comparison and object a in the comparison throwing null ref exception
            if (item != null)
                item.IsSelected = true;
            else
                Debug.LogWarning(itemData + " is not amongst the items",gameObject);
            //items.ForEach(i => i.IsSelected = (i.Data == itemData || (i.IsSelected && multipleSelectionMax > 1)));//if (i.Data.Equals(item)) i.IsSelected = true; });
        }

        public bool IsSelected(K itemData) {
            T item = items.Find(i => object.Equals(i.Data, itemData)); //To avoid ref comparison and object a in the comparison throwing null ref exception
            return item != null && item.IsSelected;
        }

        public void Up() {
            if (items.Count <= 0)
                return;

            int selectedIndex = SelectedIndex;
            T selectedItem;
            if (selectedIndex == 0) {
                items[0].IsSelected = false;
                selectedItem = items[items.Count - 1];
            } else if (selectedIndex > 0) {
                items[selectedIndex].IsSelected = false;
                selectedItem = items[selectedIndex - 1];
            } else
                selectedItem = items[0];

            selectedItem.IsSelected = true;
            FireSelectionChangedEvent(selectedItem);
        }

        public void Down() {
            if (items.Count <= 0)
                return;

            int selectedIndex = SelectedIndex;
            T selectedItem;
            if (selectedIndex >= 0 && selectedIndex < items.Count)
                items[selectedIndex].IsSelected = false;

            if (selectedIndex >= 0 && selectedIndex < items.Count - 1) {
                selectedItem = items[selectedIndex + 1];
            }
            else if (selectedIndex > 0)
                selectedItem = items[items.Count - 1];
            else
                selectedItem = items[0];

            selectedItem.IsSelected = true;
            FireSelectionChangedEvent(selectedItem);
        }

        private void FireSelectionChangedEvent(T item) {
            //SnapTo (GetSelectedIndex ());
            EnsureItemFullVisibility(items.Find(i => i.IsSelected));

            if (OnItemSelectionChange != null)
                OnItemSelectionChange(item);
            if (OnListSelectionChange != null)
                OnListSelectionChange();
        }

        protected virtual void EnsureItemFullVisibility(T item) {
            if (scrollRect == null || item == null)
                return;

            float scrollRectSize = scrollRect.GetComponent<RectTransform>().rect.height;

            float minY = System.Math.Max(0, item.size.y / 2 - scrollRectSize - item.anchoredPosition.y);
            float maxY = -item.anchoredPosition.y - item.size.y / 2;

            Vector2 contentPos = content.anchoredPosition;
            if (contentPos.y < minY || contentPos.y > maxY) {

                contentPos.y = System.Math.Min(System.Math.Max(contentPos.y, minY), maxY);

                content.anchoredPosition = contentPos;
            }
        }

        public void SnapTo(int index) {
            if (scrollRect == null || index < 0)
                return;

            Canvas.ForceUpdateCanvases();

            content.anchoredPosition = (Vector2)scrollRect.transform.InverseTransformPoint(content.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(items[index].anchoredPosition);
        }

    }
}
