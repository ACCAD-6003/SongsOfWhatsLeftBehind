/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as scriptable object that can be used to have a shared event between objects and pass an object
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Generic Event", menuName = "Shared Data/Generic Event")]
    public class SharedGenericEvent : ScriptableObject
    {
        public Action<object> Value
        {
            get => OnTrigger;
            set => OnTrigger = value;
        }

        private event Action<object> OnTrigger;

        [ContextMenu("Trigger Event")]
        public void Trigger(object obj)
        {
            OnTrigger?.Invoke(obj);
        }
    }
}