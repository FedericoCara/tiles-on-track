using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI{

	public class Panel : MonoBehaviour {

		[SerializeField]
		private Image notInteractableMask;

		public virtual bool Interactable{
			get{ return notInteractableMask == null || notInteractableMask.gameObject.activeSelf; }
			set{ 
				if (notInteractableMask != null) {
					notInteractableMask.gameObject.SetActive (!value);
				}
			}
		}
	}
}
