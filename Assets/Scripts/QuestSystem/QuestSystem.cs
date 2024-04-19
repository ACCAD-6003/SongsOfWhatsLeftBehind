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
            if (TryHandleNewQuest()) return;
            if (NeedResetQuest()) return;

            while (currentQuestLine != null
                   && currentQuestLine.CheckNextCompletionStatus(WorldState.GetState))
            {
                currentQuestLine.CompleteCurrentTask();
            }
        }

        private bool NeedResetQuest()
        {
            if (!WorldState.InState("questNeedsReset")) return false;
            WorldState.SetState("questNeedsReset", 0);
            currentQuestLine.ResetQuestLine();
            return true;
        }
        
        private bool TryHandleNewQuest()
        {
            if (currentQuestID == WorldState.GetState("questType")) return false;
            if (questLines.All(x => x.TriggerEvent != WorldState.GetState("questType"))) return false;
            currentQuestID = WorldState.GetState("questType");
            currentQuestLine = questLines.Find(x => x.TriggerEvent == currentQuestID);
            Debug.Log("Swapping to quest " + currentQuestID + " quest.");
            currentQuestLine.ResetQuestLine();

            return true;
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
        
        public void CloseUI()
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