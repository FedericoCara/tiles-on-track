using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Mimic.UI{

	[RequireComponent(typeof(Toggle))]
	public class DropdownMenu : MonoBehaviour {
		
		private Toggle toggle;
        
        private string valueString;
        public string ValueString {
            get { return valueString; }
            set {
                valueString = value;
                dontTriggerListeners = true;
                contentListView.Select(value);
                dontTriggerListeners = false;
            }
        }
        private bool dontTriggerListeners = false;

        [SerializeField]
        private Text valueTxt;

        [SerializeField]
        private Transform contentContainer;

		[SerializeField]
		private ListViewString contentListView;

        public delegate void OnDropdownSet();
        public event OnDropdownSet onDropdownSetListeners;

		private void Awake(){
            toggle = GetComponent<Toggle> ();
			toggle.onValueChanged.AddListener(OnValueChanged);
		}

        private void OnValueChanged(bool value){
            contentContainer.gameObject.SetActive (value);
        }

        public void OnValueSelected(ListItemString value) {
            if (contentListView.SelectedItem == null) {
                value.IsSelected = true;
                toggle.isOn = false;
            } else if (valueTxt.text != value.Data) {
                valueTxt.text = value.Data;
                valueString = value.Data;
                toggle.isOn = false;
                if(!dontTriggerListeners)
                    onDropdownSetListeners?.Invoke();
            }
        }

        public void SetItems(List<string> items) {
            ListItemString listItemString;
            items.ForEach(item => {
                listItemString = contentListView.Add(item);
            });
            contentListView.OnItemSelectionChange += OnValueSelected;
            OnValueChanged(false);
        }
    }
}
