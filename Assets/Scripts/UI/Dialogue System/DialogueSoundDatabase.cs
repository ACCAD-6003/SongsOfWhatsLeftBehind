using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static UI.Dialogue_System.DialogueHelperClass;

namespace UI.Dialogue_System
{
    [CreateAssetMenu(menuName = "SO/Sound Database", fileName = "New Sound File")]
    public class DialogueSoundDatabase : SerializedScriptableObject
    {
        [SerializeField] Dictionary<string, AudioClip> converter = new Dictionary<string, AudioClip>();

        public AudioClip GetClip(string fileName)
        {
            if(converter.TryGetValue(fileName, out var clip))
            {
                return clip;
            }

            if (fileName != EMPTY_MARKER)
            {
                Debug.LogWarning("WAS UNABLE TO FIND " + fileName + " IN AUDIO DATABASE");
            }

            return null;
        }

        public bool GetClip(string fileName, out AudioClip clip)
        {
            clip = GetClip(fileName);
            return clip != null;
        }
    }
}
