using UnityEngine;

namespace Mimic {

    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject {

        protected static T instance;
        public static T Instance {
            get {
                if(instance == null) {
                    instance = Resources.LoadAll<T>(string.Empty)[0];
                }
                return instance;
            }
        }
    }

}
