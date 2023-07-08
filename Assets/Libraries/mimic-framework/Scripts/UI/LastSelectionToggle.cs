using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Mimic.UI{

	[RequireComponent(typeof(Button))]
	public class LastSelectionToggle : MonoBehaviour {

		public enum State{
			NON_SELECTED,
			SELECTED,
			LAST_SELECTED
		}

		public delegate void OnSelectionChanged(bool lastSelected);

		/// <summary>
		/// Graphic the toggle should be working with for selection.
		/// </summary>
		public Image selectedImg;

		/// <summary>
		/// Graphic the toggle should be working with for last selection.
		/// </summary>
		public Image lastSelectedImg;

		private Button button;

		// group that this toggle can belong to
		[SerializeField]
		private LastSelectionToggleGroup group;
		public LastSelectionToggleGroup Group{
			get{ return group; }
		}

		private OnSelectionChanged onSelectionChangedDelegate;
		public OnSelectionChanged OnSelectionChangedDelegate{
			set{ onSelectionChangedDelegate = value; }
		}

		private State m_State;
		public State state{
			get{ return m_State; }
			internal set{ 	
				if (m_State == value)
					return;
				
				m_State = value; 
				selectedImg.enabled = m_State == State.SELECTED;
				lastSelectedImg.enabled = m_State == State.LAST_SELECTED;

				if (onSelectionChangedDelegate != null)
					onSelectionChangedDelegate (m_State == State.LAST_SELECTED);
			}
		}

		public bool interactable{
			get{ return button.interactable; }
			set{ button.interactable = value; }
		}

		private void Start(){
			button = GetComponent<Button> ();
			button.onClick.AddListener(() => group.OnSelectionChanged(this));
			selectedImg.enabled = m_State == State.SELECTED;
			lastSelectedImg.enabled = m_State == State.LAST_SELECTED;
		}

		public void SetAsLastSelected(){
			if(m_State == State.NON_SELECTED){
				group.OnSelectionChanged(this);
			} else if(state == State.SELECTED) {
				group.OnSelectionChanged(this);
				group.OnSelectionChanged(this);
			}
		}
	}
}

