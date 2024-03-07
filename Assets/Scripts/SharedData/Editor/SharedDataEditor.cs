/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2022, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEditor;
using UnityEngine;

namespace SharedData.Editor
{
    /// <summary>
    ///     Makes it so that you can broadcast the value of a SharedData<> object from the inspector.
    /// </summary>
    [CustomEditor(typeof(SharedData<>), true)]
    public class SharedDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Broadcast Value") && target is IInvokable myTarget) myTarget.Invoke();
        }
    }
}