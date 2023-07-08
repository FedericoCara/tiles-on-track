using System;
using UnityEngine;

namespace Mimic.Pooling {

	public interface IPoolable {
        
        string PoolKey { get; set; }

        void Initialize(); //what should the object do when is ready to be reused
        void OnPutBackInPool(); //called when the object is on the pool again
    }
	
}
