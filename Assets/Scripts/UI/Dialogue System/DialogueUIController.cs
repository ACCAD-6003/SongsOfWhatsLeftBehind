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
        [SerializeField] private ConversantType player;
        [SerializeField] private Dictionary<ConversantType, DialogueDisplay> dialogueBackgrounds;
        [SerializeField] private Image grayBackground;
        [SerializeField] private Image playerPortrait;
        
        private void OnEnable()
        {
            DialogueManager.OnDialogueStarted += DisplayUI;
            DialogueManager.OnDialogueEnded += HideUI;
            HideUI();
        }

        private void HideUI()
        {
            DialogueManager.OnTextUpdated -= UpdateDialogue;
            Debug.Log("Hiding UI");
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        
            textBoxDisplay.Hide();
        }

        private void DisplayUI(ConversationData conversation, ConversantType playerWhoEnteredDialogue)
        {
            if (player != playerWhoEnteredDialogue) return;
            Debug.Log("Displaying UI");
        
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        
            textBoxDisplay.Display(player);
            DialogueManager.OnTextUpdated += UpdateDialogue;
            DialogueManager.OnTextSet += SetDialogue;
        }

        private void SetDialogue(string text, ConversantType playerListener, ConversantType speaker)
        {
            if (player != playerListener) return;
            
            grayBackground.enabled = DialogueManager.Instance.InInternalDialogue || speaker == ConversantType.Other;
            playerPortrait.enabled = DialogueManager.Instance.InInternalDialogue;
            textBoxDisplay.UpdateDialogueText("", playerListener);
            textBoxDisplay.SetDialogueText(text, playerListener);
            dialogueBackgrounds.Values.Select(x => x.background).ForEach(x => x.SetActive(false));
            dialogueBackgrounds[speaker].background.SetActive(true);
            textBoxDisplay.SwapDialogueTextField(dialogueBackgrounds[speaker].textField);
        }
        
        private void UpdateDialogue(string text, ConversantType playerListener, ConversantType speaker)
        {
            if (player != playerListener) return;
            textBoxDisplay.UpdateDialogueText(text, playerListener);
        }

        private void OnDisable()
        {
            DialogueManager.OnDialogueStarted -= DisplayUI;
            DialogueManager.OnDialogueEnded -= HideUI;
            DialogueManager.OnTextSet -= SetDialogue;
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueStarted -= DisplayUI;
            DialogueManager.OnTextUpdated -= UpdateDialogue;
            DialogueManager.OnTextSet -= SetDialogue;
        }

        [System.Serializable]
        public class DialogueDisplay
        {
            public TMP_Text textField;
            public GameObject background;
        }
    }
}