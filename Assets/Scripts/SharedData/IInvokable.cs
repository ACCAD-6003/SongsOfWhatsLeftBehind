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
    ///     Interface for an object that can be invoked
    /// </summary>
    public interface IInvokable
    {
        public void Invoke();
    }
}