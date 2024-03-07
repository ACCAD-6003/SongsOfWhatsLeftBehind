/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2022, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using System.Collections.Generic;

namespace SharedData.SaveSystem
{
    /// <summary>
    /// Represents the list of types of data that can be saved through the save system
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public List<NestedList<string>> stringLists;
        public List<string> strings;
        public List<int> ints;
        public List<bool> booleans;
    }
}