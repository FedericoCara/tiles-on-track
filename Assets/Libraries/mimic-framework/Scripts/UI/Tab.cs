using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI {
	
	public class Tab : XToggle {

		[SerializeField]
		private List<Text> titleTxts;
		public string Title{
			set{ titleTxts.ForEach (t => t.text = value); }
		}

		[SerializeField]
		private GameObject content;
		public GameObject Content{
			set{ Utils.ReplaceChildrenWithGO (content.transform, value); }
		}

		protected override void Awake(){			
			base.Awake();
			onValueChanged.AddListener (OnToggleClick);
		}

		private void OnToggleClick(bool enable){
			if(content != null) {
				content.SetActive (isOn);
			}
		}
	}

}

