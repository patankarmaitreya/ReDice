using System.Collections.Generic;
using UnityEngine;

namespace AssociativeFiles.Custom
{
    public class WeightedRNG
    {
        private int minValue;
        private int maxValue;
        public float probability;

        public WeightedRNG() { }

        //Always Enter in Increasing Order of probability
        public void SetWeightedRNG(int minValue, int maxValue, float probability)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.probability = probability;
        }

        public int GetValue() => UnityEngine.Random.Range(minValue, maxValue);
    }

    public static class CalcWeightedRNG
    {
        public static int GetRandomValue(List<WeightedRNG> selections)
        {
            float rand = UnityEngine.Random.value;
            float currentProb = 0;
            foreach (var selection in selections)
            {
                currentProb += (selection.probability) / 100;                                 //be sure to change the values from like 0.6 to 60
                if (rand <= currentProb)
                    return selection.GetValue();
            }
            return -1;
        }
    }
}
