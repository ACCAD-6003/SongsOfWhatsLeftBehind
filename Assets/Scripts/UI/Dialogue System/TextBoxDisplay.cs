using TMPro;
using UnityEngine;

namespace UI.Dialogue_System
{
    public class TextBoxDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text dialogueTextField;

        private DialogueHelperClass.ConversantType conversant;

        public void Display(DialogueHelperClass.ConversantType conversantToShow)
        {
            ToggleChildrenDisplay(true);
            conversant = conversantToShow;
        }

        public void SetDialogueText(string text, DialogueHelperClass.ConversantType playerListening)
        {
            if (playerListening != conversant) return;
            dialogueTextField.text = text;
        }
        
        public void UpdateDialogueText(string text, DialogueHelperClass.ConversantType playerListening)
        {
            if (playerListening != conversant) return;
            dialogueTextField.maxVisibleCharacters = text.Length;
        }
        
        public void SwapDialogueTextField(TMP_Text newDialogueTextField)
        {
            newDialogueTextField.text = dialogueTextField.text;
            dialogueTextField = newDialogueTextField;
        }

        public void Hide() => ToggleChildrenDisplay(false);

        private void ToggleChildrenDisplay(bool shouldDisplay)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldDisplay);
            }
        }
    }
}