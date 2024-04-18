using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UI.Dialogue_System.DialogueHelperClass;

namespace UI.Dialogue_System
{
    public class DialogueUIController : SerializedMonoBehaviour
    {
        [SerializeField] private TextBoxDisplay textBoxDisplay;
        [SerializeField] private ChoicesDisplay choicesDisplay;
        
        private void OnEnable()
        {
            DialogueManager.OnDialogueStarted += DisplayUI;
            DialogueManager.OnDialogueEnded += HideUI;
            HideUI();
        }

        private void HideUI()
        {
            DialogueManager.OnTextUpdated -= UpdateDialogue;
            DialogueManager.OnChoiceMenuOpen -= DisplayChoices;
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        
            textBoxDisplay.Hide();
        }

        private void DisplayUI(ConversationData conversation)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        
            textBoxDisplay.Display();
            DialogueManager.OnTextUpdated -= UpdateDialogue;
            DialogueManager.OnTextSet -= SetDialogue;
            DialogueManager.OnChoiceMenuOpen -= DisplayChoices;
            
            DialogueManager.OnTextUpdated += UpdateDialogue;
            DialogueManager.OnTextSet += SetDialogue;
            DialogueManager.OnChoiceMenuOpen += DisplayChoices;
        }

        private void SetDialogue(DialogueData dialogue)
        {
            textBoxDisplay.UpdateDialogueText("");
            textBoxDisplay.SetDialogueText(dialogue.speakerName, dialogue.Dialogue);
        }
        
        private void UpdateDialogue(string text)
        {
            textBoxDisplay.UpdateDialogueText(text);
        }
        
        private void DisplayChoices(List<string> choices)
        {
            choicesDisplay.Display(choices, OnChoiceSelected);
            textBoxDisplay.SetDialogueText("", "");
        }
        
        private void OnChoiceSelected(int index)
        {
            DialogueManager.Instance.SelectChoice(index);
            choicesDisplay.Hide();
        }

        private void OnDisable()
        {
            DialogueManager.OnDialogueStarted -= DisplayUI;
            DialogueManager.OnDialogueEnded -= HideUI;
            DialogueManager.OnTextSet -= SetDialogue;
            DialogueManager.OnTextUpdated -= UpdateDialogue;
            DialogueManager.OnChoiceMenuOpen -= DisplayChoices;
        }

        [System.Serializable]
        public class DialogueDisplay
        {
            public TMP_Text textField;
            public GameObject background;
        }
    }
}