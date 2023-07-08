using System;

namespace Mimic.Pooling {
	
	public interface IPoolUser {
        //Better call it on Start so it doesn't interfere with scene changed pool clenaup
		void InitializePool();
	}

}

