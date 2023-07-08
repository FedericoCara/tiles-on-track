using UnityEngine;
using System.Collections.Generic;

using Mimic;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Mimic.Pooling {
	
	public class PoolManager : AutoInstantiableSingleton<PoolManager> {

		[SerializeField]
		private int refillAmount = 10;

        private Hashtable pools = new Hashtable();
		
		protected virtual void Start() {
            SceneManager.activeSceneChanged += OnSceneChanged;
			DontDestroyOnLoad (gameObject);
		}

		/// <summary>
		/// Creates a pool of instances of many prefabs that will be treated as the same. 
		/// They will all share the key of the first IPoolable prefab.
		/// When you ask for an objects, you can receive any of the instances.
		/// </summary>
		/// <typeparam name="T">Type of the prefab</typeparam>
		/// <param name="prefabs">List of prefabs to be instantiated</param>
		/// <param name="initialAmount">Initial amount for the pool</param>
		/// <param name="checkRepeatedObjects">When putting back an object, should it check if it already is present in the pool</param>
		public Pool<T> CreatePool<T>(List<T> prefabs, int initialAmount = 10, bool checkRepeatedObjects = true) where T : MonoBehaviour, IPoolable {
			return CreatePool<T>(prefabs[0].PoolKey, prefabs, initialAmount, checkRepeatedObjects);
		}

		/// <summary>
		/// Creates a pool of instances of many prefabs that will be treated as the same. 
		/// They will all share the same key.
		/// When you ask for an objects, you can receive any of the instances.
		/// </summary>
		/// <typeparam name="T">Type of the prefab</typeparam>
		/// <param name="key">Key that will identify the pool</param>
		/// <param name="prefabs">List of prefabs to be instantiated</param>
		/// <param name="initialAmount">Initial amount for the pool</param>
		/// <param name="checkRepeatedObjects">When putting back an object, should it check if it already is present in the pool</param>
		public Pool<T> CreatePool<T>(string key, List<T> prefabs, int initialAmount = 10, bool checkRepeatedObjects = true) where T : MonoBehaviour, IPoolable {
			if (pools.ContainsKey(key)) {
				Pool<T> pool = GetPool<T>(key);
				pool.CompleteUpTo(initialAmount);
				return pool;
			} else {
				return AddPool(key, prefabs, initialAmount, checkRepeatedObjects);
			}
		}

		/// <summary>
		/// Creates a pool of instances of the prefab.
		/// The key will be the the one from the IPoolable object
		/// </summary>
		/// <typeparam name="T">Type of the prefab</typeparam>
		/// <param name="prefab">Prefab to be instantiated</param>
		/// <param name="initialAmount">Initial amount for the pool</param>
		/// <param name="checkRepeatedObjects">When putting back an object, should it check if it already is present in the pool</param>
		public void CreatePool<T>(T prefab, int initialAmount = 10, bool checkRepeatedObjects = true) where T : MonoBehaviour, IPoolable {
			CreatePool<T>(prefab.PoolKey, prefab, initialAmount, checkRepeatedObjects);
		}

		/// <summary>
		/// Creates a pool of instances of the prefab.
		/// </summary>
		/// <typeparam name="T">Type of the prefab</typeparam>
		/// <param name="key">Key that will identify the pool</param>
		/// <param name="prefab">Prefab to be instantiated</param>
		/// <param name="initialAmount">Initial amount for the pool</param>
		/// <param name="checkRepeatedObjects">When putting back an object, should it check if it already is present in the pool</param>
		public Pool<T> CreatePool<T>(string key, T prefab, int initialAmount = 10, bool checkRepeatedObjects = true) where T : MonoBehaviour, IPoolable {
			List<T> prefabList = new List<T>();
			prefabList.Add(prefab);
			return CreatePool<T>(key, prefabList, initialAmount, checkRepeatedObjects);
		}

		private Pool<T> AddPool<T>(string key, List<T> prefabs, int initialAmount, bool checkRepeatedObjects) where T : MonoBehaviour, IPoolable {
			GameObject poolGO = new GameObject(key + " Pool");
			poolGO.transform.SetParent(transform, false);
			Pool<T> pool = new Pool<T>(poolGO.transform, checkRepeatedObjects);
			int i = 0;
			pool.Init(initialAmount, () => {
				T newObject = Instantiate<T>(prefabs[i], pool.TransformRelated);
				if (string.IsNullOrEmpty(newObject.PoolKey)) {
					newObject.PoolKey = key;
				}

				newObject.name = key + "-id" + newObject.GetInstanceID();
				i = (i + 1) % prefabs.Count;

				return newObject;
			});
			pools.Add(key, pool);
			return pool;
		}

		private Pool<T> AddPool<T>(string key, T prefab, int initialAmount, bool checkRepeatedObjects) where T : MonoBehaviour, IPoolable {
			List<T> prefabList = new List<T>();
			prefabList.Add(prefab);
			return AddPool<T>(key, prefabList, initialAmount, checkRepeatedObjects);
		}

		public void IncreasePool<T>(string key, int amount) where T : MonoBehaviour, IPoolable {
            GetPool<T>(key).AddAmount(amount);
		}

		public void PutBackInPool<T>(T returnedObject) where T : MonoBehaviour, IPoolable {
			Pool<T> pool = GetPool<T>(returnedObject.PoolKey);
			pool?.PutBackInPool (returnedObject);
		}

		public T GetObject<T>(string key) where T : MonoBehaviour, IPoolable {
            Pool<T> pool = GetPool<T>(key);
            T newObject = (T) pool.GetObject();
			if(pool.IsEmpty) {
				pool.AddAmount(refillAmount);
			}
            newObject.gameObject.SetActive(true);
			return newObject;
		}

        private Pool<T> GetPool<T>(string key) where T : MonoBehaviour, IPoolable {
            return (Pool<T>)pools[key];
        }

        protected void OnSceneChanged(Scene current, Scene next){
            IPool pool;
            foreach (DictionaryEntry pair in pools) {
                pool = (IPool)pair.Value;
                pool.DestroyTransform();
            }
            pools.Clear();
        }
	}
}

