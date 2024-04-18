using UnityEngine;

namespace UI.Dialogue_System
{
    public class HideOnDialogue : MonoBehaviour
    {
        private void OnEnable()
        {
            DialogueManager.OnDialogueStarted += Hide;
            DialogueManager.OnDialogueEnded += Show;
        }

        private void Hide(DialogueHelperClass.ConversationData _)
        {
            foreach (Transform child in transform) child.gameObject.SetActive(false);
        }

        private void Show()
        {
            foreach (Transform child in transform) child.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            DialogueManager.OnDialogueStarted -= Hide;
            DialogueManager.OnDialogueEnded -= Show;
        }
    }
}
