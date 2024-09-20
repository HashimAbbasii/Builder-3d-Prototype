using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtraStaticFunctions
{

    #region Probability

    public struct RandomSelection
    {
        private float _minValue;
        private float _maxValue;
        public float Probability { get; private set; }

        public RandomSelection(int minValue, int maxValue, float probability)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            Probability = probability;
        }

        public RandomSelection(float minValue, float maxValue, float probability)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            Probability = probability;
        }
        
        public RandomSelection(int singleValue, float probability)
        {
            _minValue = singleValue;
            _maxValue = singleValue;
            Probability = probability;
        }
        
        public RandomSelection(float singleValue, float probability)
        {
            _minValue = singleValue;
            _maxValue = singleValue;
            Probability = probability;
        }


        public float GetValue()
        {
            return Random.Range(_minValue, _maxValue);
        }

    }
    
    public static float GetRandomValue(params RandomSelection[] selections)
    {
        float rand = Random.value;
        float currentProb = 0;
        foreach (var selection in selections)
        {
            currentProb += selection.Probability;
            if (rand <= currentProb)
                return selection.GetValue();
        }

        return -1;
    }
    
    #endregion

    #region Remap Values

        public static float RemapValue(float value, float inMin, float inMax, float outMin, float outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

    #endregion
    
}


