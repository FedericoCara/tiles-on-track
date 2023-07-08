using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI{

	public class Dialog : MonoBehaviour {

		protected Dictionary<string, Action> buttonDownActions = new Dictionary<string, Action>();

		protected virtual void Update() {
			foreach(string key in buttonDownActions.Keys){
				if(InputAdapter.GetButtonDown(key)) {
					buttonDownActions[key]();
				}
			}
		}

		public void AddButtonDownAction(string button, Action action){
			buttonDownActions.Add(button, action);
		}
	}
}
