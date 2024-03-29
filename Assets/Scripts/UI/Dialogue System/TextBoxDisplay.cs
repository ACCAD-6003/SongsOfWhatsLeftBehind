using System.Linq;
using TMPro;
using UnityEngine;

namespace UI.Dialogue_System
{
    public class TextBoxDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text dialogueTextField;
        [SerializeField] TMP_Text nameTextField;
        [SerializeField] private GameObject nameField;
        [SerializeField] private GameObject continueIndicator;

        private DialogueHelperClass.ConversantType conversant;

        public void Display(DialogueHelperClass.ConversantType conversantToShow)
        {
            ToggleChildrenDisplay(true);
            conversant = conversantToShow;
        }

        public void SetDialogueText(string text, DialogueHelperClass.ConversantType playerListening)
        {
            if (playerListening != conversant) return;
            var label = text.Split('\n')[0];
            nameTextField.text = label.Contains(':') ? label.Split(':')[0] : "";
            nameField.SetActive(nameTextField.text != "");
            dialogueTextField.text = label.Contains(':') ? text.Replace(label, "").Trim('\n') : text;
            dialogueTextField.maxVisibleCharacters = 0;
            continueIndicator.SetActive(false);
        }
        
        public void UpdateDialogueText(string text, DialogueHelperClass.ConversantType playerListening)
        {
            if (playerListening != conversant) return;
            dialogueTextField.maxVisibleCharacters = text.Length;
            continueIndicator.SetActive(dialogueTextField.text != "" &&
                dialogueTextField.maxVisibleCharacters >= dialogueTextField.text.Length);
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