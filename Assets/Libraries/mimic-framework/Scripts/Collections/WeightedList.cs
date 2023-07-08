using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Mimic.Collections {

    [Serializable]
    public class WeightedList<T> : List<WeightedValue<T>> {

        public T GetRandomWeighted() {
            float totalWeights = 0;
            ForEach(weightedValue => totalWeights += weightedValue.Weight);

            float randomValue = Random.value * totalWeights;
            int i = -1;
            while(randomValue > 0) {
                randomValue -= this[++i].Weight;
            }
            return this[i].Value;
        }

    }

}