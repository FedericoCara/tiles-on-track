using UnityEngine;

namespace Mimic.Pooling
{
    public class SimplePoolable : MonoBehaviour, IPoolable {
        
        [SerializeField]
        private string poolKey;

        public string PoolKey {
            get => poolKey;
            set => poolKey = value;
        }

        #region IPoolObject implementation
        public void Initialize()
        {
            
        }

        public void OnPutBackInPool()
        {
            
        }
        #endregion
    }
}