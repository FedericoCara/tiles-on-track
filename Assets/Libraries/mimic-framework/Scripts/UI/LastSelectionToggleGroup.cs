using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mimic.UI{

	public class LastSelectionToggleGroup : MonoBehaviour {

		private List<LastSelectionToggle> lastSelectedStack = new List<LastSelectionToggle>();
		public List<LastSelectionToggle> Selection{
			get{ return lastSelectedStack; }
		}

		public void DeselectAll(){
			lastSelectedStack.ForEach (t => t.state = LastSelectionToggle.State.NON_SELECTED);
			lastSelectedStack.Clear ();
		}

		internal void OnSelectionChanged(LastSelectionToggle toggle){
			int indexOf = lastSelectedStack.IndexOf (toggle);
			if (indexOf < 0) {
				lastSelectedStack.ForEach (t => t.state = LastSelectionToggle.State.SELECTED);
				lastSelectedStack.Insert (0, toggle);
				toggle.state = LastSelectionToggle.State.LAST_SELECTED;
			} else {
				lastSelectedStack.RemoveAt (indexOf);
				toggle.state = LastSelectionToggle.State.NON_SELECTED;
				if (indexOf == 0 && lastSelectedStack.Count > 0) {
					lastSelectedStack [0].state = LastSelectionToggle.State.LAST_SELECTED;
				}
			}
		}

	}
}
