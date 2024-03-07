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
    ///     This serves an object that will notify subscribers when it's value changes
    /// </summary>
    public interface IShareableData : IInvokable
    {
        public event Action OnValueChanged;
        public string ValueAsText();
    }
}