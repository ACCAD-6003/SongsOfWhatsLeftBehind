using UnityEngine;
using static UI.Dialogue_System.DialogueHelperClass;

namespace UI.Dialogue_System
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogueSoundListener : MonoBehaviour
    {
        [SerializeField] DialogueSoundDatabase soundDatabase;
        new AudioSource audio;

        private void Start()
        {
            audio = GetComponent<AudioSource>();
            DialogueManager.OnDialogueStarted += TriggerAudio;
        }

        private void TriggerAudio(ConversationData dialogueNode, ConversantType _)
        {
            if (soundDatabase.GetClip(dialogueNode.Conversant, out var clip))
            {
                audio.PlayOneShot(clip);
            }
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueStarted -= TriggerAudio;
        }
    }
}
