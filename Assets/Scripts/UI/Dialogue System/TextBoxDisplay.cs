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

        public void Display()
        {
            ToggleChildrenDisplay(true);
        }

        public void SetDialogueText(string speakerName, string dialogue)
        {
            nameTextField.text = speakerName;
            nameField.SetActive(nameTextField.text != "");
            dialogueTextField.maxVisibleCharacters = 0;
            dialogueTextField.text = dialogue;
            continueIndicator.SetActive(false);
        }
        
        public void UpdateDialogueText(string text)
        {
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