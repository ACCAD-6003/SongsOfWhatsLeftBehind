using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using static UI.Dialogue_System.DialogueHelperClass;

namespace UI.Dialogue_System
{
    public class DialogueBoxColorHandler : SerializedMonoBehaviour
    {
        [SerializeField] private Image dialogueBox;
        [SerializeField] private Dictionary<string, Color> colors;
        [SerializeField] private Color defaultColor = Color.white;
        
        private void OnEnable()
        {
            DialogueManager.OnTextSet += SetColor;
        }
        
        private void SetColor(DialogueData dialogue)
        {
            var characterName = dialogue.speakerName;
            dialogueBox.color = colors.GetValueOrDefault(characterName, defaultColor);
        }
        
        private void OnDisable()
        {
            DialogueManager.OnTextSet -= SetColor;
        }
    }
}