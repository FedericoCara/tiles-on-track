using System;
using UnityEngine;

namespace Mimic.Pooling {
	
	public class PoolingParticle: MonoBehaviour, IPoolable
	{
		[SerializeField]
		private ParticleSystem mainParticle;

		[SerializeField, Range(0f,1f)]
		private float startTime;

        [SerializeField]
        private float duration = -1;

		private string keyName;
		public string PoolKey {
			get {
				return keyName;
			}
			set {
				keyName = value;
			}
		}

		void Awake(){
			if(mainParticle==null){
				mainParticle = GetComponent<ParticleSystem> ();
			}
		}

        public void PutBackInPool() {
            PoolManager.Instance.PutBackInPool<PoolingParticle>(this);
        }

		#region IPoolObject implementation
		public void Initialize () {
			mainParticle.Simulate (startTime, true, true);
			mainParticle.Play (true);
            if (duration > 0) {
                Invoke(nameof(PutBackInPool), duration);
            }
		}

		public void OnPutBackInPool () {
		}

		#endregion

	}
	
}

