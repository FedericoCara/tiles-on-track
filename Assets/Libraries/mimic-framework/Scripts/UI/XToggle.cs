using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Mimic.UI{

	[AddComponentMenu("UI/XToggle", 33)]
	[RequireComponent(typeof(RectTransform))]
	public class XToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement	{

		/// <summary>
		/// Transition type.
		/// </summary>
		public Toggle.ToggleTransition toggleTransition = Toggle.ToggleTransition.Fade;

		/// <summary>
		/// Graphic the toggle should be active while isOn = true.
		/// </summary>
		[SerializeField, UnityEngine.Serialization.FormerlySerializedAs("graphicsParent")]
		protected GameObject graphicsParentWhileIsOn;

		/// <summary>
		/// Graphic the toggle should be active while isOn = false.
		/// </summary>
		[SerializeField]
		protected GameObject graphicsParentWhileIsNotOn;

		/// <summary>
		/// Graphic the toggle should be active while isOn = false.
		/// </summary>
		[SerializeField]
		protected bool activateDeactivateGameObjects = false;


		// group that this toggle can belong to
		[SerializeField]
		private XToggleGroup m_Group;

		public XToggleGroup group
		{
			get { return m_Group; }
			set
			{
				m_Group = value;
				#if UNITY_EDITOR
				if (Application.isPlaying)
				#endif
				{
					SetToggleGroup(m_Group, true);
					PlayEffect(true);
				}
			}
		}

		/// <summary>
		/// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
		/// </summary>
		public Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();

		// Whether the toggle is on
		[FormerlySerializedAs("m_IsActive")]
		[Tooltip("Is the toggle currently on or off?")]
		[SerializeField]
		private bool m_IsOn;

		[Tooltip("Call onValueChanged at Rebuild")]
		[SerializeField]
		private bool callOnValueChangedAtRebuild = true;

		protected XToggle()
		{}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			Set(m_IsOn, false);
			PlayEffect(toggleTransition == Toggle.ToggleTransition.None);

			var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
			if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		#endif // if UNITY_EDITOR

		public virtual void Rebuild(CanvasUpdate executing)
		{
			#if UNITY_EDITOR
			if (callOnValueChangedAtRebuild && executing == CanvasUpdate.Prelayout)
				onValueChanged.Invoke(m_IsOn);
			#endif
		}

		public virtual void LayoutComplete()
		{}

		public virtual void GraphicUpdateComplete()
		{}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetToggleGroup(m_Group, false);
			PlayEffect(true);
		}

		protected override void OnDisable()
		{
			SetToggleGroup(null, false);
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			// Check if isOn has been changed by the animation.
			// Unfortunately there is no way to check if we don�t have a graphic.
			if (graphicsParentWhileIsOn != null)
			{
				Graphic graphic = graphicsParentWhileIsOn.GetComponentInChildren<Graphic> ();
				if(graphic != null){
					bool oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
					if (m_IsOn != oldValue)
					{
						m_IsOn = oldValue;
						Set(!oldValue);
					}
				}
			}

			base.OnDidApplyAnimationProperties();
		}

		private void SetToggleGroup(XToggleGroup newGroup, bool setMemberValue)
		{
			XToggleGroup oldGroup = m_Group;

			// Sometimes IsActive returns false in OnDisable so don't check for it.
			// Rather remove the toggle too often than too little.
			if (m_Group != null)
				m_Group.UnregisterToggle(this);

			// At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
			// That's why we use the setMemberValue parameter.
			if (setMemberValue)
				m_Group = newGroup;

			// Only register to the new group if this Toggle is active.
			if (m_Group != null && IsActive())
				m_Group.RegisterToggle(this);

			// If we are in a new group, and this toggle is on, notify group.
			// Note: Don't refer to m_Group here as it's not guaranteed to have been set.
			if (newGroup != null && newGroup != oldGroup && isOn && IsActive())
				m_Group.NotifyToggleOn(this);
		}

		/// <summary>
		/// Whether the toggle is currently active.
		/// </summary>
		public bool isOn
		{
			get { return m_IsOn; }
			set
			{
				Set(value);
			}
		}

		/// <summary>
        /// Set isOn without invoking onValueChanged callback.
        /// </summary>
        /// <param name="value">New Value for isOn.</param>
        public void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }

		void Set(bool value)
		{
			Set(value, true);
		}

		void Set(bool value, bool sendCallback)
		{
			if (m_IsOn == value)
				return;

			// if we are in a group and set to true, do group logic
			m_IsOn = value;
			if (m_Group != null && IsActive())
			{
				if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
				{
					m_IsOn = true;
					m_Group.NotifyToggleOn(this);
				}
			}

			if(activateDeactivateGameObjects) {
				graphicsParentWhileIsOn.SetActive(m_IsOn);
				graphicsParentWhileIsNotOn.SetActive(!m_IsOn);
			}

			// Always send event when toggle is clicked, even if value didn't change
			// due to already active toggle in a toggle group being clicked.
			// Controls like Dropdown rely on this.
			// It's up to the user to ignore a selection being set to the same value it already was, if desired.
			PlayEffect(toggleTransition == Toggle.ToggleTransition.None);
			if (sendCallback)
				onValueChanged.Invoke(m_IsOn);
		}

		/// <summary>
		/// Play the appropriate effect.
		/// </summary>
		private void PlayEffect(bool instant)
		{
			PlayEffect(graphicsParentWhileIsOn, instant, m_IsOn ? 1f : 0f);
			PlayEffect(graphicsParentWhileIsNotOn, instant, m_IsOn ? 0f : 1f);
		}

		private static void PlayEffect(GameObject graphicsParent, bool instant, float targetAlpha)
		{
			if (graphicsParent == null)
				return;

			Graphic[] graphics = graphicsParent.GetComponentsInChildren<Graphic> ();

			if (graphics == null)
				return;

			for(int i=0; i<graphics.Length; i++){
				#if UNITY_EDITOR
				if (!Application.isPlaying)
					graphics[i].canvasRenderer.SetAlpha(targetAlpha);
				else
				#endif
					graphics[i].CrossFadeAlpha(targetAlpha, instant ? 0f : 0.1f, true);
			}
		}

		/// <summary>
		/// Assume the correct visual state.
		/// </summary>
		protected override void Start()
		{
			PlayEffect(true);
		}

		private void InternalToggle()
		{
			if (!IsActive() || !IsInteractable())
				return;

			isOn = !isOn;
		}

		/// <summary>
		/// React to clicks.
		/// </summary>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			InternalToggle();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			InternalToggle();
		}
	}
}