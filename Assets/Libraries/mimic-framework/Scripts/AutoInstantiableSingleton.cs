using UnityEngine;

namespace Mimic {

    public class AutoInstantiableSingleton<T> : Singleton<T> where T : AutoInstantiableSingleton<T> {

		public new static T Instance {
			get {
				T instance = Singleton<T>.Instance;
				if (instance == null) {
					GameObject singletonGO = new GameObject(typeof(T).Name);
					instance = singletonGO.AddComponent<T>();
					if(!instance.DestroyOnLoad) {
						DontDestroyOnLoad(instance);
					}
				}
				return instance;
			}
		}

		public virtual bool DestroyOnLoad => true;

	}
}
