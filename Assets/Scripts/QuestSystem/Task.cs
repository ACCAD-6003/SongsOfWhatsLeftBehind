using System;

namespace QuestSystem
{
    [Serializable]
    public class Task
    {
        public string label;
        public string completionCondition;
        
        public Task(string label, string completionCondition)
        {
            this.label = label;
            this.completionCondition = completionCondition;
        }
    }
}