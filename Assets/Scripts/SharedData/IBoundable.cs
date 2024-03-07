/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

namespace SharedData
{
    /// <summary>
    ///     Serves as a generic interface for a class that has a lower and upper bound
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBoundable<out T>
    {
        public T LowerBound { get; }
        public T UpperBound { get; }
    }
}