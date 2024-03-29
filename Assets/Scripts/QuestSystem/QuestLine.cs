using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    [CreateAssetMenu(fileName = "New Quest Line", menuName = "Quest System/Quest Line")]
    public class QuestLine : ScriptableObject
    {
        [SerializeField] private string questName;
        [SerializeField] private List<Task> tasks = new();

        private int currentTaskIndex = 0;
        
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
        
        public string GetNextTaskCondition()
        {
            return currentTaskIndex < tasks.Count ? tasks[currentTaskIndex].completionCondition : null;
        }

        public void CompleteCurrentTask()
        {
            currentTaskIndex++;
        }
    }
}