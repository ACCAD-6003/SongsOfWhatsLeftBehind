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
    ///     This serves as scriptable object that can be used to have a shared bool between objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Bool", menuName = "Shared Data/Bool")]
    public class SharedBool : SharedData<bool>
    {
        [SerializeField] private bool value;

        public override bool Value
        {
            get => value;
            set
            {
                this.value = value;
                BroadcastValueChanged();
            }
        }

        [ContextMenu("Flip boolean")]
        public void Flip()
        {
            Value = !Value;
        }
    }
}