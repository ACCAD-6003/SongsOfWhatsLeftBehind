using UnityEngine;

namespace UI.Dialogue_System
{
    public class TriggerDialogueOnStart : MonoBehaviour
    {
        [SerializeField] private string conversation;
        
        private void Start()
        {
            DialogueManager.Instance.StartDialogueName(conversation);
        }
    }
}