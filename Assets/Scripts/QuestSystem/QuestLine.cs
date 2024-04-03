using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace QuestSystem
{
    [CreateAssetMenu(fileName = "New Quest Line", menuName = "Quest System/Quest Line")]
    public class QuestLine : ScriptableObject
    {
        [SerializeField] private string questName;
        [SerializeField] private int triggerEvent;
        [SerializeField] private List<Task> tasks = new();
        
        private int currentTaskIndex = 0;
        
        public int TriggerEvent => triggerEvent;

        public void SetupQuestLine(string sectionToParse)
        {
            var lines = sectionToParse.Split('\n').Where(x => !x.IsNullOrWhitespace()).ToArray();
            triggerEvent = int.Parse(lines[0].Trim());
            questName = lines[1].Trim();
            name = "Quest" + triggerEvent;
            tasks.Clear();
            for (int i = 2; i < lines.Length; i++)
            {
                tasks.Add(new Task(lines[i]));
            }
        }

        public void ResetQuestLine()
        {
            currentTaskIndex = 0;
        }

        public string GetTaskDescription()
        {
            var description = "> " + questName;
            for (int i = 0; i < currentTaskIndex; i++)
            {
                description += $"\n    -<s>{tasks[i].label}</s>";
            }
            if (currentTaskIndex < tasks.Count)
            {
                description += $"\n    -{tasks[currentTaskIndex].label}";
            }

            return description;
        }
        
        public bool CheckNextCompletionStatus(Func<string, int> getWorldState)
        {
            return currentTaskIndex < tasks.Count && tasks[currentTaskIndex].IsCompleted(getWorldState);
        }

        public void CompleteCurrentTask()
        {
            currentTaskIndex++;
        }
    }
}