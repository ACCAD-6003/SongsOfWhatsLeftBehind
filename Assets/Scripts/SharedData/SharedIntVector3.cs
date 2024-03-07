/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;

namespace SharedData
{
    /// <summary>
    /// Represents the a Vector3 of shared ints which each represent a component of that vector3
    /// </summary>
    [Serializable]
    public class SharedIntVector3
    {
        public SharedData<int> x;
        public SharedData<int> y;
        public SharedData<int> z;
    }
}