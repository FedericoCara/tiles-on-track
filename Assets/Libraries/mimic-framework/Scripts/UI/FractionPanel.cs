using UnityEngine;
using UnityEngine.UI;

using Mimic.UI;
using System.Collections.Generic;
using Mimic.Math;

namespace Mimic.UI {

    public class FractionPanel : MonoBehaviour {

        [SerializeField]
        protected Graphic fractionNegativeSign;
        [SerializeField]
        protected Toggle signToggle;

        [SerializeField]
        protected Text numeratorTxt;

        [SerializeField]
        protected DropdownMenu numeratorDropdown;

        [SerializeField]
        protected Graphic editableFractionDivider;
        [SerializeField]
        protected Graphic nonEditableFractionDivider;

        [SerializeField]
        protected Text denominatorTxt;

        [SerializeField]
        protected DropdownMenu denominatorDropdown;

        [SerializeField]
        protected Text notFractionTxt;
        

        [SerializeField]
        protected GameObject editableFraction;

        [SerializeField]
        protected GameObject nonEditableFraction;

        [SerializeField]
        private float integerWidth;
        [SerializeField]
        private float fixedFractionWidth;
        [SerializeField]
        private float editableFractionWidth;

        public delegate void OnCoefficientSet();
        public event OnCoefficientSet onCoefficientSetListeners;

        private bool editable = false;
        public bool Editable {
            get {
                return editable;
            }
            set {
                editable = value;
                if (fraction != null)
                    UpdateFraction(fraction);
            }
        }


        protected virtual void Start() {
            InitializeInteractables();
        }

        protected virtual void InitializeInteractables() {
            string[] denominatorArray = new string[] { "1", "2", "3", "4", "5" };
            List<string> numeratorItems = new List<string>(denominatorArray), denominatorItems = new List<string>(denominatorArray);
            numeratorItems.Insert(0, "0");
            numeratorDropdown.SetItems(numeratorItems);
            denominatorDropdown.SetItems(denominatorItems);
            numeratorDropdown.onDropdownSetListeners += () => {
                fraction = ExtractFractionFromDropdowns();
                onCoefficientSetListeners?.Invoke();
            };
            denominatorDropdown.onDropdownSetListeners += () => {
                fraction = ExtractFractionFromDropdowns();
                onCoefficientSetListeners?.Invoke();
            };
            signToggle.onValueChanged.AddListener(OnSignChanged);
        }

        public void OnSignChanged(bool value) {
            fraction = ExtractFractionFromDropdowns();
            onCoefficientSetListeners?.Invoke();
        }

        protected Fraction fraction;
        public Fraction Fraction {
            get { return fraction; }
            set {
                if (fraction != value) {
                    UpdateFraction(value);
                }
            }
        }

        private Fraction ExtractFractionFromDropdowns() {
            return new Coefficient((signToggle.isOn ? 1 : -1) * int.Parse(numeratorDropdown.ValueString), int.Parse(denominatorDropdown.ValueString), ((Coefficient)fraction).Color);
        }

        protected virtual void UpdateFraction(Fraction value) {
            if (editableFraction != null) {
                editableFraction.SetActive(false);
            }
            RectTransform rt = GetComponent<RectTransform>();
            if (!editable && value.Denominator==1) {
                notFractionTxt.text = value.Value.ToString();
                notFractionTxt.gameObject.SetActive(true);
                nonEditableFraction.SetActive(false);
                rt.sizeDelta = new Vector2(integerWidth, rt.sizeDelta.y);
            } else if (!editable) {
                numeratorTxt.text = Mathf.Abs(value.Numerator).ToString();
                denominatorTxt.text = Mathf.Abs(value.Denominator).ToString();
                notFractionTxt.gameObject.SetActive(false);
                fractionNegativeSign.gameObject.SetActive(value.Value < 0);
                nonEditableFraction.SetActive(true);
                rt.sizeDelta = new Vector2(fixedFractionWidth, rt.sizeDelta.y);
            } else {
                numeratorDropdown.ValueString = Mathf.Abs(value.Numerator).ToString();
                denominatorDropdown.ValueString = Mathf.Abs(value.Denominator).ToString();
                signToggle.SetIsOnWithoutNotify(value.Value >= 0);
                notFractionTxt.gameObject.SetActive(false);
                nonEditableFraction.SetActive(false);
                editableFraction.SetActive(true);
                rt.sizeDelta = new Vector2(editableFractionWidth, rt.sizeDelta.y);
            }
            fraction = value;
            UpdateParentLayout();
        }

        private void UpdateParentLayout() {
            HorizontalLayoutGroup parentLayout = GetComponentInParent<HorizontalLayoutGroup>();
            parentLayout.enabled = false;
            parentLayout.enabled = true;
        }

        public Color Color {
            set {
                fractionNegativeSign.color = value;
                numeratorTxt.color = value;
                nonEditableFractionDivider.color = value;
                editableFractionDivider.color = value;
                denominatorTxt.color = value;
                notFractionTxt.color = value;
            }
        }
    }
}