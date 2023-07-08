using System;

using UnityEngine;

namespace Mimic.Collections {

    [Serializable]
    public class WeightedValue<T> {
        
        [SerializeField]
        private float weight;
        public float Weight => weight;

        [SerializeField]
        private T value;
        public T Value => value;

    }
    

}