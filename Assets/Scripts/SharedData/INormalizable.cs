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
    ///     Implementors of the INormalizable interface are able to be given a normalized value to have their value
    ///     set to and the ability to return their value on a normalized scale.
    /// </summary>
    public interface INormalizable
    {
        /// <summary>
        ///     Given a value from zero to one, it will set the value of the implementor to be within its min and max
        ///     values respective to the normalized field.
        /// </summary>
        /// <param name="normalizedValue"></param>
        public void SetFromNormal(float normalizedValue);

        /// <summary>
        ///     Will return the value of the implementor normalized to its min and max to be a value between 0 and 1
        /// </summary>
        public float GetNormal();

        /// <summary>
        ///     Will return a string representation of the implementor so that it can be used for debugging potential issues
        /// </summary>
        /// <returns></returns>
        public string DebugInformation();
    }
}