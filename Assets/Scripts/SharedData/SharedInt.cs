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
    ///     This serves as scriptable object that can be used to have a shared int between objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Int", menuName = "Shared Data/Normalizable/Integer")]
    public class SharedInt : SharedData<int>, INormalizable, IBoundable<int>
    {
        [SerializeField] private int minValue;
        [SerializeField] private int maxValue;
        [SerializeField] private int value;

        public override int Value
        {
            get => value;
            set
            {
                this.value = Mathf.Clamp(value, minValue, maxValue);
                BroadcastValueChanged();
            }
        }

        public int LowerBound => minValue;
        public int UpperBound => maxValue;

        public void SetFromNormal(float normalizedValue)
        {
            Value = Mathf.RoundToInt(minValue + normalizedValue * (maxValue - minValue));
        }

        public float GetNormal()
        {
            return (value - minValue) / (float)(maxValue - minValue);
        }

        public string DebugInformation()
        {
            return $"Min Value: {minValue} | Max Value: {maxValue} | Value: {value} | Normalized Value: {GetNormal()}";
        }
    }
}