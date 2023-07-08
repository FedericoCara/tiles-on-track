using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mimic;

namespace Mimic.UI {

    public class EnsureAspectRatio : MonoBehaviour {

        [SerializeField]
        private float targetAspectRatio;

        [SerializeField]
        private float threshold = 0.001f;

        [SerializeField]
        private Color stripesColor = Color.black;

        [SerializeField]
        private bool overrideSorting = false;

        [SerializeField, ConditionalField(nameof(overrideSorting))]
        private string sortingLayerName;

        [SerializeField, ConditionalField(nameof(overrideSorting))]
        private int orderInLayer;

        private Vector2 size;
        public Vector2 Size => size;

        private CanvasScaler canvasScaler;

        private RectTransform rectTransform;

        private RectTransform ensuredAspectRatioPnl;
        public Vector2 EnsuredSize => ensuredAspectRatioPnl!=null ? ensuredAspectRatioPnl.sizeDelta : Size;

        private RectTransform topStripe;

        private RectTransform bottomStripe;

        private RectTransform leftStripe;

        private RectTransform rightStripe;

        private List<Action<Vector2>> onSizeChangedActions = new List<Action<Vector2>>();

        protected virtual void Awake() {
            rectTransform = GetComponent<RectTransform>();
            canvasScaler = GetComponent<CanvasScaler>();
            OnSizeChanged();
        }

        protected virtual void Update() {            
            if(rectTransform.sizeDelta != size) {
                OnSizeChanged();
            }            
        }

        private void OnSizeChanged() {
            size = rectTransform.sizeDelta;
            float currentAspectRatio = size.x / size.y; 

            // If the current aspect ratio is the same as the target
            // we don't need to do anything else
            if(Mathf.Abs(currentAspectRatio - targetAspectRatio) < threshold) {
                if(ensuredAspectRatioPnl != null) {
                    if(topStripe != null) {
                        topStripe.gameObject.SetActive(false);
                        bottomStripe.gameObject.SetActive(false);
                    }
                    if(leftStripe != null) {
                        leftStripe.gameObject.SetActive(false);
                        rightStripe.gameObject.SetActive(false);
                    }
                    ensuredAspectRatioPnl.sizeDelta = size;
                }
                FireOnSizeChangedActions(size);
                return;
            } 

            Debug.Log("Ensuring Aspect Ratio...");

            // Check if we need to create the center parent
            if(ensuredAspectRatioPnl == null) {

                List<Transform> children = rectTransform.GetChildren();

                // First let's create the center panel
                ensuredAspectRatioPnl = new GameObject("Ensured Aspect Ratio Pnl", typeof(RectTransform)).GetComponent<RectTransform>();
                ensuredAspectRatioPnl.SetParent(rectTransform, false);
                ensuredAspectRatioPnl.transform.localScale = Vector3.one;
                ensuredAspectRatioPnl.sizeDelta = rectTransform.sizeDelta;

                children.ForEach(child => child.SetParent(ensuredAspectRatioPnl, false));
            } 

            Vector2 targetSize = size;         
            Vector2 stripeSize = size;   
            if(currentAspectRatio > targetAspectRatio) {
                targetSize.x = targetSize.y * targetAspectRatio;  
                stripeSize.x = (size.x - targetSize.x) / 2;           

                if(leftStripe == null) {
                    leftStripe = CreateStripe("Left Stripe Img", new Vector2(0, 0.5f));
                    rightStripe = CreateStripe("Right Stripe Img", new Vector2(1, 0.5f));
                } else {
                    leftStripe.gameObject.SetActive(true);
                    rightStripe.gameObject.SetActive(true);
                }

                leftStripe.sizeDelta = stripeSize;
                rightStripe.sizeDelta = stripeSize;

                if(topStripe != null) {
                    topStripe.gameObject.SetActive(false);
                    bottomStripe.gameObject.SetActive(false);
                }

                if(canvasScaler != null) {
                    canvasScaler.matchWidthOrHeight = 1;
                }
            } else {
                targetSize.y = targetSize.x / targetAspectRatio;
                stripeSize.y = (size.y - targetSize.y) / 2;    

                if(topStripe == null) {
                    topStripe = CreateStripe("Top Stripe Img", new Vector2(0.5f, 1));
                    bottomStripe = CreateStripe("Bottom Stripe Img", new Vector2(0.5f, 0));
                } else {
                    topStripe.gameObject.SetActive(true);
                    bottomStripe.gameObject.SetActive(true);
                }

                topStripe.sizeDelta = stripeSize;
                bottomStripe.sizeDelta = stripeSize;

                if(leftStripe != null) {
                    leftStripe.gameObject.SetActive(false);
                    rightStripe.gameObject.SetActive(false);
                }

                if(canvasScaler != null) {
                    canvasScaler.matchWidthOrHeight = 0;
                }
            }

            ensuredAspectRatioPnl.sizeDelta = targetSize;   
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            FireOnSizeChangedActions(targetSize);
        }   

        private RectTransform CreateStripe(string name, Vector2 anchor) {
            GameObject stripeGO = new GameObject(name, typeof(RectTransform));            
            stripeGO.AddComponent<Image>().color = stripesColor;
            RectTransform stripeTransform = stripeGO.GetComponent<RectTransform>();
            stripeTransform.SetParent(rectTransform);
            stripeTransform.transform.localPosition = Vector3.zero;
            stripeTransform.transform.localScale = Vector3.one;
            stripeTransform.anchorMin = anchor;
            stripeTransform.anchorMax = anchor;
            stripeTransform.pivot = anchor;

            if(overrideSorting) {
                Canvas canvas = stripeGO.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingLayerName = sortingLayerName;
                canvas.sortingOrder = orderInLayer;
            }

            return stripeTransform;
        }

        public Vector2 WorldToEnsuredViewportPoint(Vector2 position) {
            float reductionX = 1 - EnsuredSize.x / Size.x;
            float reductionY = 1 - EnsuredSize.y / Size.y;
            Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(position);
            viewPortPoint.x += reductionX / 2;
            viewPortPoint.y += reductionY / 2;
            return viewPortPoint;
        }

        public void AddOnSizeChangedAction(Action<Vector2> onSizeChanged) {
            onSizeChangedActions.Add(onSizeChanged);
        }

        private void FireOnSizeChangedActions(Vector2 newSize) {
            onSizeChangedActions.ForEach(action => action(newSize));
        }
        
    }

}
