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
    /// A shared pointer to an object
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Pointer", menuName = "Shared Data/Pointer")]
    public class SharedPointer : SharedData<object>
    {
        private object value;

        public override object Value
        {
            get => value;
            set
            {
                this.value = value;
                BroadcastValueChanged();
            }
        }

        public T ValueAs<T>()
        {
            Debug.Assert(Value is T,
                $"Expected {typeof(T)} but did not get it", this);
            return (T)Value;
        }
        
        public static T ValueAs<T>(object obj)
        {
            Debug.Assert(obj is T,
                $"Expected {typeof(T)} but did not get it from {obj}");
            return (T)obj;
        }
    }
}