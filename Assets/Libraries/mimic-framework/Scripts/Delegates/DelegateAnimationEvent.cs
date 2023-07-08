using UnityEngine;
using System.Collections;

namespace Mimic.Animations{
	public class DelegateAnimationEvent : MonoBehaviour
	{
		[SerializeField]
		private GameObject target;

		[SerializeField]
		private char separationChar = ',';

		void OnAnimationEvent(string methodName){
			if (target != null) {
				string[] methodAndParameter = methodName.Split(separationChar);
				if (methodAndParameter.Length > 1) {
					string method = methodAndParameter[0];
					string parameter = methodAndParameter[1];
					target.SendMessage(method, parameter, SendMessageOptions.DontRequireReceiver);
				} else {
					target.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
				}
			} else {
				Debug.LogWarning("target is null", gameObject);
			}
		}
	}
}