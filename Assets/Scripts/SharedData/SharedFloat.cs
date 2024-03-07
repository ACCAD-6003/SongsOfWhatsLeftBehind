/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as scriptable object that can be used to have a shared float between objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Float", menuName = "Shared Data/Normalizable/Float")]
    public class SharedFloat : SharedData<float>, INormalizable, IClamped<float>
    {
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;
        [SerializeField] private float value;
        [SerializeField] private int sigFigs = -1;

        public float MinValue => minValue;
        public float MaxValue => maxValue;
        

        public override float Value
        {
            get => value;
            set
            {
                this.value = Mathf.Clamp(value, minValue, maxValue);
                BroadcastValueChanged();
            }
        }


        public void SetFromNormal(float normalizedValue)
        {
            Value = minValue + normalizedValue * (maxValue - minValue);
        }

        public float GetNormal()
        {
            return (Value - minValue) / (maxValue - minValue);
        }

        public string DebugInformation()
        {
            return $"Min Value: {minValue} | Max Value: {maxValue} | Value: {Value} | Normalized Value: {GetNormal()}";
        }

        public void SetMax(float newMax)
        {
            maxValue = newMax;
        }
        
        public void SetMin(float newMin)
        {
            minValue = newMin;
        }

        public override string ToString()
        {
            return sigFigs == -1 ? value.ToString() : value.ToString($"F{sigFigs}");
        }
    }
}