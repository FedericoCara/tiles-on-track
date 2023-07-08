using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mimic;

namespace Mimic.UI{

	/// <summary>
	/// This is a base multi-purpose dialog class to display and handle several modals
	/// like information modal, confirmation modal, input modal, waiting message modal, etc.
	/// </summary>
	public class OptionDialog : Singleton<OptionDialog> {

        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("innerPanel")]
        private GameObject innerPnl;

        public virtual string TextString {
            get { return Text.text; }
            set {
                if (value != null) {
                    Text.text = value;
                } else {
                    Text.text = "";
                }
            }
        }

        [SerializeField]
		private Text text;
		public Text Text{
			get{ return text; }
		}

        public virtual GameObject TextGO {
            get { return text.gameObject; }
        }

		[SerializeField, UnityEngine.Serialization.FormerlySerializedAs("inputField")]
		private InputField inputFld;
		public InputField InputFld {
			get{ return inputFld; }
		}

		[SerializeField]
		private List<Button> options;
		public List<Button> Options{
			get{ return options; }
		}

		[SerializeField]
		private float minTimeWaintingMessageDisplayed = 1f;

        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("closeButton")]
        private Button closeBtn;
		public Action OnCloseBtnClickAction {
			set{
				if(closeBtn != null) {
					SetAction(closeBtn, value);
					closeBtn.gameObject.SetActive(value != null);
				}
			}
		}

        [SerializeField]
        private bool triggerOnStart = true;

		private float waitingMessageTime;
		private bool IsWaitingMessageActive;
		private bool closeWaitingMessage;

		private Action onClosedWaitingMessageAction;

		protected virtual void Awake(){
			instance = this;
		}

		protected virtual void Start(){
            if(triggerOnStart) {
			    options.ForEach (o => SetAction(o, null));
			}
		}

		protected virtual void Update(){
			if (IsWaitingMessageActive) {
				waitingMessageTime += Time.deltaTime;
				if (closeWaitingMessage && waitingMessageTime > minTimeWaintingMessageDisplayed) {
					IsWaitingMessageActive = false;
                    innerPnl.SetActive(false);
					if (onClosedWaitingMessageAction != null) {
						onClosedWaitingMessageAction ();
						onClosedWaitingMessageAction = null;
					}
				}	
			}
		}

		public void ShowWaitingMessage(){
			ShowWaitingMessage ("Please Wait...");
		}

		public void ShowWaitingMessage(string message){
			TextGO.SetActive (true);
			inputFld?.gameObject.SetActive(false);
			TextString = message;
			TurnOnOptionsUntil (0);
            innerPnl.SetActive (true);
			waitingMessageTime = 0;
			IsWaitingMessageActive = true;
			closeWaitingMessage = false;
            OnCloseBtnClickAction = null;
        }

        public void CloseWaitingMessage(Action onClosedWaitingMessageAction = null){		
			closeWaitingMessage = true;
			this.onClosedWaitingMessageAction = onClosedWaitingMessageAction;
		}
		
		public void ShowErrorMessage(string message, Action onOkAction = null){
			ShowInformationMessage (message, onOkAction);
		}
		
		public void ShowInformationMessage(string message){
			ShowInformationMessage (message, null);
		}
		
		public virtual void ShowInformationMessage(string message, Action onOkAction){
			TextGO.SetActive (true);
			inputFld?.gameObject.SetActive(false);
			TextString = message;
			SetOption (0,  HashIDs.OK_BTN_TEXT, onOkAction);
			TurnOnOptionsUntil (1);
            innerPnl.SetActive (true);
            OnCloseBtnClickAction = null;
        }

        public void ShowConfirmationMessage(string message, Action onOkAction){
			ShowConfirmationMessage(message, onOkAction, null);
		}

		public void ShowConfirmationMessage(string message, Action onOkAction, Action onCancelAction, Action onCloseBtnClickAction = null) {
			TextGO.SetActive (true);
			inputFld?.gameObject.SetActive(false);
			TextString = message;
			SetOption (0,  HashIDs.OK_BTN_TEXT, onOkAction);
			SetOption (1,  HashIDs.CANCEL_BTN_TEXT, onCancelAction);
			TurnOnOptionsUntil (2);
            OnCloseBtnClickAction = onCloseBtnClickAction;

            innerPnl.SetActive (true);
		}

		public void ShowInputDialog(string label, Action onOkAction, Action onCancelAction, Action onCloseBtnClickAction = null) {
			TextGO.SetActive (true);
			inputFld.gameObject.SetActive (true);
			TextString = label;
			SetOption (0,  HashIDs.OK_BTN_TEXT, onOkAction);
			SetOption (1,  HashIDs.CANCEL_BTN_TEXT, onCancelAction);
			TurnOnOptionsUntil (2);
			innerPnl.SetActive (true);
            OnCloseBtnClickAction = onCloseBtnClickAction;
		}

		public virtual void ShowOptionMessage(string message, Action onOption1Action, Action onOption2Action, Action onOption3Action = null, Action onCloseBtnClickAction = null) {
			TextGO.SetActive (true);
			inputFld?.gameObject.SetActive(false);
			TextString = message;

			SetAction (0,  onOption1Action);
			SetAction (1,  onOption2Action);
			if(onOption3Action != null){
				SetAction (2, onOption3Action);
				TurnOnOptionsUntil (3);
			} else {
				TurnOnOptionsUntil (2);
			}

            OnCloseBtnClickAction = onCloseBtnClickAction;
            innerPnl.SetActive (true);

        }

        public void ShowOptionMessage(string message, params KeyValuePair<string, Action>[] options){
			ShowOptionMessage (message, new List<KeyValuePair<string, Action>> (options));
		}

		public void ShowOptionMessage(string message, List<KeyValuePair<string, Action>> options, Action onCloseBtnClickAction = null){
			TextGO.SetActive (true);
			inputFld?.gameObject.SetActive(false);
			TextString = message;

			SetOptions (options);
            OnCloseBtnClickAction = onCloseBtnClickAction;

            innerPnl.SetActive (true);
		}

		public void SetOptionsText(params string[] optionsText){
			for (int i = 0; i < optionsText.Length; i++) {
				SetText (i, optionsText[i]);
			}
		}

		public void SetOptions(params KeyValuePair<string, Action>[] options){
			SetOptions (new List<KeyValuePair<string, Action>> (options));
		}

		public void SetOptions(List<KeyValuePair<string, Action>> options){
			for (int i = 0; i < options.Count; i++) {
				if (i == this.options.Count) {
					Button newOption = Instantiate<Button> (this.options [0]);
					newOption.transform.SetParent (this.options [0].transform.parent, false);
					this.options.Add (newOption);
				}
				SetOption (i, options [i].Key, options [i].Value);
			}
			TurnOnOptionsUntil (options.Count);
		}
			
		public string GetInputText(){
			return inputFld == null ? "" : inputFld.textComponent.text;
		}

		public void SetActive(bool visible){
            innerPnl.SetActive (visible);
		}

		protected void SetOption(int index, string text, Action onClickAction){
			SetText (index, text);
			SetAction (index, onClickAction);
		}

		private void SetText(int index, string text){
			options [index].GetComponentInChildren<Text> ().text = text;
		}

		private void SetAction(int index, Action onClickAction){
			SetAction(options [index], onClickAction);
		}

		private void SetAction(Button button, Action onClickAction){
			button.onClick.RemoveAllListeners ();
			button.onClick.AddListener( () => {
				SetActive (false);
				onClickAction?.Invoke();
			});
		}

		private void TurnOnOptionsUntil(int index){
			int i = 0;
			options.ForEach (o => o.gameObject.SetActive (i++ < index));
		}
	}
}
