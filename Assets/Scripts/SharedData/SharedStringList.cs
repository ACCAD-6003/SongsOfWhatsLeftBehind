/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.Collections.Generic;
using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as scriptable object that can be used to have a shared list of strings between objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Strings List", menuName = "Shared Data/List/Strings")]
    public class SharedStringList : SharedList<string>
    {
        [SerializeField] private List<string> elements;

        protected override List<string> Elements
        {
            get => elements;
            set => elements = value;
        }
    }
}