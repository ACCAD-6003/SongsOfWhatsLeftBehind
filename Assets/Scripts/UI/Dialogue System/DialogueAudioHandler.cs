using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Dialogue_System
{
    public class DialogueAudioHandler : SerializedMonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Dictionary<string, float> characterPitches;
        [SerializeField] private float defaultPitch = 1;
        [SerializeField] private AudioClip blipSound;
        [SerializeField] private float variation = 0.2f;
        [SerializeField] private int frequency = 3;

        private float pitch = 1;
        
        private void OnEnable()
        {
            DialogueManager.OnTextSet += SetBlip;
            SetBlip(DialogueManager.Instance.CurrentDialogue);
            DialogueManager.OnTextUpdated += PlayBlip;
        }
        
        private void SetBlip(DialogueHelperClass.DialogueData dialogue)
        {
            if (dialogue == null) return;
            var characterName = dialogue.speakerName;
            pitch = characterPitches.GetValueOrDefault(characterName, defaultPitch);
        }
        
        private void PlayBlip(string text)
        {
            if (text.Length % frequency != 0) return;
            audioSource.pitch = Random.Range(-1f, 1f) * variation + pitch;
            audioSource.PlayOneShot(blipSound);
        }
        
        private void OnDisable()
        {
            DialogueManager.OnTextSet -= SetBlip;
            DialogueManager.OnTextUpdated -= PlayBlip;
        }
    }
}