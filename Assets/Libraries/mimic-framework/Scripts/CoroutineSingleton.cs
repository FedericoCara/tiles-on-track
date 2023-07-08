using UnityEngine;
using System.Collections;

namespace Mimic{

	public class CoroutineSingleton : MonoBehaviour
	{
		private static CoroutineSingleton instance;
		public static CoroutineSingleton Instance
		{
			get
			{
				if(instance==null){
					instance = FindObjectOfType<CoroutineSingleton>();
					if(instance==null){
						instance = new GameObject("CoroutineSingleton").AddComponent<CoroutineSingleton>();
					}
				}
				return instance;
			}
		}
	}
}

