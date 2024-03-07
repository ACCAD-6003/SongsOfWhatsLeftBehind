/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as the interface for a scriptable object data type that allows for sharing of variables
    ///     between game objects.
    /// </summary>
    public abstract class SharedData<T> : ScriptableObject, IShareableData
    {
        public abstract T Value { get; set; }
        public virtual event Action OnValueChanged;

        [Button("Broadcast Value")]
        public void Invoke()
        {
            BroadcastValueChanged();
        }

        public virtual string ValueAsText()
        {
            return Value.ToString();
        }

        protected void BroadcastValueChanged()
        {
            OnValueChanged?.Invoke();
        }

        public static implicit operator T(SharedData<T> x)
        {
            return x.Value;
        }
    }
}