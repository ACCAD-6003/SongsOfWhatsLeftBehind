using System;
using System.Collections.Generic;
using System.Linq;
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
        
        [Button]
        public void EditorCreateColorDictionary()
        {
            colors = new Dictionary<string, Color>();
            Resources.LoadAll<SOConversationData>("Dialogue")
                    .SelectMany(x => x.Data.DialoguesSeries)
                    .SelectMany(y => y.dialogues)
                    .Select(z => z.speakerName)
                    .Distinct()
                    .ToList().ForEach(x => colors.Add(x, defaultColor));
        }
        
        private void OnEnable()
        {
            DialogueManager.OnTextSet += SetColor;
            DialogueManager.OnChoiceMenuOpen += SetColorForChoices;
            SetColor(DialogueManager.Instance.CurrentDialogue);
        }
        
        private void SetColorForChoices(List<string> choices)
        {
            dialogueBox.color = colors.GetValueOrDefault(PLAYER_MARKER.Split(":")[0], defaultColor);
        }
        
        private void SetColor(DialogueData dialogue)
        {
            if (dialogue == null) return;
            var characterName = dialogue.speakerName;
            dialogueBox.color = colors.GetValueOrDefault(characterName, defaultColor);
        }
        
        private void OnDisable()
        {
            DialogueManager.OnTextSet -= SetColor;
            DialogueManager.OnChoiceMenuOpen -= SetColorForChoices;
        }
    }
}