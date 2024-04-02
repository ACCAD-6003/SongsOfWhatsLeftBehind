using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace QuestSystem
{
    [Serializable]
    public class Task
    {
        public string label;
        private string completionCondition;
        private ComparisonType comparisonType;
        private int comparisonValue;
        public enum ComparisonType { GreaterThan, LessThan, EqualTo, NotEqualTo, GreaterThanOrEqualTo, LessThanOrEqualTo }
        
        /*
         * This constructor is used by the Unity Editor to create new tasks
         * The format of the text is
         * {Task Label} => {Completion Condition} {Comparison Type} {Comparison Value}
         * where comparison type is one of the following: >, <, ==, !=, >=, <=
         * Example: "Befriend the shopkeeper | ShopkeeperFriendship >= 100"
         * or
         * {Task Label} | {Completion Condition}
         * Example: "Drink some milk | MilkDrank"
         */
        public Task(string line)
        {
            var components = line.Split("=>");
            label = components[0].Trim();
            var condition = components[1].Trim();
            var comparisionFound = comparisonTypeMap.Select(x => x.Key).FirstOrDefault(x => condition.Contains(x));

            if (comparisionFound == null)
            {
                comparisonType = ComparisonType.GreaterThanOrEqualTo;
                comparisonValue = 1;
                completionCondition = condition;
            }
            else
            {
                comparisonType = comparisonTypeMap[comparisionFound];
                var comparisonComponents = condition.Split(comparisionFound);
                Debug.Log(condition);
                completionCondition = comparisonComponents[0].Trim();
                comparisonValue = int.Parse(comparisonComponents[1].Trim());
            }
        }

        public bool IsCompleted(Func<string, int> getState)
        {
            var value = getState(completionCondition);
            return comparisonType switch
            {
                ComparisonType.GreaterThan => value > comparisonValue,
                ComparisonType.LessThan => value < comparisonValue,
                ComparisonType.EqualTo => value == comparisonValue,
                ComparisonType.NotEqualTo => value != comparisonValue,
                ComparisonType.GreaterThanOrEqualTo => value >= comparisonValue,
                ComparisonType.LessThanOrEqualTo => value <= comparisonValue,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private Dictionary<string, ComparisonType> comparisonTypeMap = new()
        {
            {" > ", ComparisonType.GreaterThan},
            {" < ", ComparisonType.LessThan},
            {" == ", ComparisonType.EqualTo},
            {" != ", ComparisonType.NotEqualTo},
            {" >= ", ComparisonType.GreaterThanOrEqualTo},
            {" <= ", ComparisonType.LessThanOrEqualTo}
        };
    }
}