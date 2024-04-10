using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Dialogue_System
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "AudioDatabase")]
    public class AudioDatabase : SerializedScriptableObject
    {
        public Dictionary<string, AudioClip> AudioClips = new();
    }
}