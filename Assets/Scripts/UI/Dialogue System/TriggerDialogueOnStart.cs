using UnityEngine;

namespace UI.Dialogue_System
{
    public class TriggerDialogueOnStart : MonoBehaviour
    {
        [SerializeField] private SOConversationData conversation;
        
        private void Start()
        {
            DialogueManager.Instance.StartDialogue(conversation);
        }
    }
}