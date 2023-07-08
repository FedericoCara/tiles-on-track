using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mimic.UI
{
	[AddComponentMenu("UI/XToggle Group", 34)]
	[DisallowMultipleComponent]
	public class XToggleGroup : UIBehaviour
	{
		[SerializeField] private bool m_AllowSwitchOff = false;
		public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

		private List<XToggle> m_Toggles = new List<XToggle>();

		protected XToggleGroup()
		{}

		private void ValidateToggleIsInGroup(XToggle toggle)
		{
			if (toggle == null || !m_Toggles.Contains(toggle))
				throw new ArgumentException(string.Format("XToggle {0} is not part of XToggleGroup {1}", new object[] {toggle, this}));
		}

		public void NotifyToggleOn(XToggle toggle)
		{
			ValidateToggleIsInGroup(toggle);

			// disable all toggles in the group
			m_Toggles.ForEach(t => {
				if(t != toggle){
					t.isOn = false;
				}
			});
		}

		public void UnregisterToggle(XToggle toggle)
		{
			//if (m_Toggles.Contains(toggle))
				m_Toggles.Remove(toggle);
		}

		public void RegisterToggle(XToggle toggle)
		{
			if (!m_Toggles.Contains(toggle))
				m_Toggles.Add(toggle);
		}

		public bool AnyTogglesOn(){
			return m_Toggles.Exists(x => x.isOn);
		}

		public List<XToggle> ActiveToggles(){
			return m_Toggles.FindAll(x => x.isOn);
		}

		public void SetAllTogglesOff()
		{
			bool oldAllowSwitchOff = m_AllowSwitchOff;
			m_AllowSwitchOff = true;

			m_Toggles.ForEach (toggle => toggle.isOn = false);

			m_AllowSwitchOff = oldAllowSwitchOff;
		}
	}
}
