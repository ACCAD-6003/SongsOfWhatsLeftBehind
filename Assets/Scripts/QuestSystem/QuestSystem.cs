using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Sirenix.OdinInspector;
using UnityEngine;
using UI.Dialogue_System;

namespace QuestSystem
{
    public class QuestSystem : SerializedMonoBehaviour
    {
        [SerializeField] private List<QuestLine> questLines = new();
        [SerializeField] QuestUIDisplay questUIDisplay;
        
        private QuestLine currentQuestLine;
        private int currentQuestID;

        private void Awake()
        {
            questUIDisplay.Hide();
        }

        private void OnEnable()
        {
            WorldState.OnWorldStateChanged += HandleWorldStateChanged;
            UIController.OnOpenQuestLog += OpenUI;
        }
        
        private void HandleWorldStateChanged()
        {
            TryHandleNewQuest("");

            if (currentQuestLine == null 
                || !currentQuestLine.CheckNextCompletionStatus(WorldState.GetState)) return;

            currentQuestLine.CompleteCurrentTask();
        }
        
        private void TryHandleNewQuest(string eventName)
        {
            if (questLines.All(x => x.TriggerEvent != eventName)) return;
            currentQuestLine = questLines.Find(x => x.TriggerEvent == eventName);
            currentQuestLine.ResetQuestLine();
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
            UIController.OnResume += CloseUI;
            UIController.OnCloseQuestLog += CloseUI;
            UIController.OnOpenQuestLog -= OpenUI;
        }
        
        private void CloseUI()
        {
            UIController.Instance.SwapToGameplay();
            questUIDisplay.Hide();
            UIController.OnResume -= CloseUI;
            UIController.OnCloseQuestLog -= CloseUI;
            UIController.OnOpenQuestLog += OpenUI;
        }
        
        private void OnDisable()
        {
            WorldState.OnWorldStateChanged -= HandleWorldStateChanged;
            UIController.OnOpenQuestLog -= OpenUI;
            UIController.OnCloseQuestLog -= CloseUI;
            UIController.OnResume -= CloseUI;
        }
    }
}