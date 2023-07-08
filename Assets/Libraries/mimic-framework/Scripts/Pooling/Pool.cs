using System.Collections.Generic;

using UnityEngine;

using Mimic;

namespace Mimic.Pooling {

    public class Pool<T> : IPool where T : MonoBehaviour, IPoolable {

        protected List<T> objects = new List<T>();

        protected PoolingType type;
        public PoolingType Type {
            get => type;
            set => type = value;
        }

        protected bool checkRepeatedObjects = true;
        public bool CheckRepeatedObjects {
            get => checkRepeatedObjects;
            set => checkRepeatedObjects = value;
        }

        public int Count {
            get{ return objects.Count; }
        }

        public bool IsEmpty {
            get{ return objects.Count == 0; }
        }

        [SerializeField]
        private bool debugOn = false;
        public bool DebugOn {
            get => debugOn;
            set => debugOn = value;
        }

        public delegate T Intantiate<T>() where T : IPoolable;

        private Intantiate<T> instantiate;
        public Transform TransformRelated { get; set; }

        public Pool(Transform transformRelated, PoolingType type = PoolingType.STACK) : this(transformRelated, true, type) {
        }

        public Pool(Transform transformRelated, bool checkRepeatedObjects, PoolingType type = PoolingType.STACK) {
            TransformRelated = transformRelated;
            this.checkRepeatedObjects = checkRepeatedObjects;
            this.type = type;
        }

        public virtual void Init(int initalAmount, Intantiate<T> instantiate) {
            this.instantiate = instantiate;
            objects.Clear();
            Utils.DestroyChildren(TransformRelated);
            AddAmount(initalAmount);
        }

        private T CreateNewInstance(bool active = false) {
            T newObject = instantiate();
            newObject.transform.SetParent(TransformRelated);
            newObject.gameObject.SetActive(active);
            return newObject;
        }

        private T AddNewInstance() {
            T newObject = CreateNewInstance();
            objects.Add(newObject);
            return newObject;
        }

        public void AddAmount(int amount) {
            for (int i = 0; i < amount; i++) {
                AddNewInstance();
			}
        }        

        public void CompleteUpTo(int amount) {
            for (int i = objects.Count - 1; i < amount; i++) {
                AddNewInstance();
            }
        }

        public void PutBackInPool(T returnedObject) {
            if (!checkRepeatedObjects || !objects.Contains(returnedObject)) {
                objects.Add(returnedObject);
                returnedObject.OnPutBackInPool();
                if (debugOn)
                    Debug.Log($"Putting object {returnedObject} back in pool", returnedObject);
            } else {
                Debug.LogError( $"{returnedObject} already on pool!", returnedObject);
            }
            returnedObject.transform.SetParent(TransformRelated);
            returnedObject.gameObject.SetActive(false);
        }

        public T GetObject() {
            if(objects.Count > 0) {
                int index;
                switch(type) {
                    case PoolingType.QUEUE: index = 0; break;
                    case PoolingType.STACK: index = objects.Count - 1; break;
                    case PoolingType.RANDOM: index = Random.Range(0, objects.Count); break;
                    default: throw new System.NotSupportedException("Pooling type not supported");
                }
                T pulledObject = objects[index];
                objects.RemoveAt(index);
                pulledObject.gameObject.SetActive(true);
                pulledObject.Initialize();
                return pulledObject;
            } else {
                return CreateNewInstance(true);
            }
        }

        public void DestroyTransform() {
            GameObject.Destroy(TransformRelated.gameObject);
        }
    }
    
}