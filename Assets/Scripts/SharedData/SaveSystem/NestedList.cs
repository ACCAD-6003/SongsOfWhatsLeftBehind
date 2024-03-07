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
    /// Allows for serializing a list of shared data since Unity does not support serializing lists of lists
    /// </summary>
    /// <typeparam name="T">The type of the elements within the list</typeparam>
    [Serializable]
    public class NestedList<T>
    {
        public List<T> list;

        public NestedList(SharedList<T> list)
        {
            this.list = list.GetCopyOfElements();
        }
    }
}