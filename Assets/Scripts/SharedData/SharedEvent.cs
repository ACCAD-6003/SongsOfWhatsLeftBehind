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
    ///     This serves as scriptable object that can be used to have a shared event between objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Event", menuName = "Shared Data/Event")]
    public class SharedEvent : ScriptableObject
    {
        public Action Value
        {
            get => OnTrigger;
            set => OnTrigger = value;
        }

        private event Action OnTrigger;

        [ContextMenu("Trigger Event")]
        public void Trigger()
        {
            OnTrigger?.Invoke();
        }
    }
}