using System;
using System.Collections.Generic;
using Controller;
using Sirenix.OdinInspector;
using UnityEngine;
using UI.Dialogue_System;

namespace QuestSystem
{
    public class QuestSystem : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<string, QuestLine> questLines = new();
        [SerializeField] QuestUIDisplay questUIDisplay;
        
        private QuestLine currentQuestLine;

        private void Awake()
        {
            questUIDisplay.Hide();
        }

        private void OnEnable()
        {
            DialogueManager.OnEventTriggered += HandleEventTriggered;
            WorldState.OnWorldStateChanged += HandleWorldStateChanged;
            UIController.OnOpenQuestLog += OpenUI;
        }
        
        private void HandleWorldStateChanged()
        {
            if (currentQuestLine == null 
                || currentQuestLine.GetNextTaskCondition() == null
                || !WorldState.InState(currentQuestLine.GetNextTaskCondition())) return;

            currentQuestLine.CompleteCurrentTask();
            if (currentQuestLine.GetNextTaskCondition() == null)
            {
                currentQuestLine = null;
            }
        }
        
        private void HandleEventTriggered(string eventName)
        {
            if (questLines.TryGetValue(eventName, out var questLine) && currentQuestLine != questLine)
            {
                questLine.ResetQuestLine();
                currentQuestLine = questLine;
            }
        }

        private void OpenUI()
        {
            if (currentQuestLine == null)
            {
                questUIDisplay.Display();
            }
            else
            {
                questUIDisplay.Display(currentQuestLine.GetTaskDescription());
            }
            
            UIController.Instance.SwapToUI();
            UIController.OnGoBack += CloseUI;
            UIController.OnCloseQuestLog += CloseUI;
            UIController.OnOpenQuestLog -= OpenUI;
        }
        
        private void CloseUI()
        {
            UIController.Instance.SwapToGameplay();
            questUIDisplay.Hide();
            UIController.OnGoBack -= CloseUI;
            UIController.OnCloseQuestLog -= CloseUI;
            UIController.OnOpenQuestLog += OpenUI;
        }
        
        private void OnDisable()
        {
            DialogueManager.OnEventTriggered -= HandleEventTriggered;
            WorldState.OnWorldStateChanged -= HandleWorldStateChanged;
            UIController.OnOpenQuestLog -= OpenUI;
            UIController.OnCloseQuestLog -= CloseUI;
            UIController.OnGoBack -= CloseUI;
        }
    }
}