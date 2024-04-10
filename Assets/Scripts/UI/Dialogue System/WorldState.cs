using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Dialogue_System
{
    public static class WorldState
    {
        private static readonly Dictionary<string, int> CurrentState = new(); 
        public static Action OnWorldStateChanged;
        
        public static void SetState(string key, int value)
        {
            if (CurrentState.TryGetValue(key, out _))
            {
                CurrentState[key] = value;
            }
            else
            {
                CurrentState[key] = value;
            }
            
            OnWorldStateChanged?.Invoke();
        }
        
        public static void SetState(string key, Func<int, int> valueCalculator)
        {
            Debug.Assert(key != null, nameof(key) + " != null");
            Debug.Assert(valueCalculator != null, nameof(valueCalculator) + " != null for key: " + key);
            CurrentState[key] = valueCalculator(CurrentState.TryGetValue(key, out var currentValue) ? currentValue : 0);
            
            OnWorldStateChanged?.Invoke();
        }
        
        public static void SetState(string key, bool value)
        {
            CurrentState[key] = value ? 1 : 0;
            
            OnWorldStateChanged?.Invoke();
        }
        
        public static int GetState(string key)
        {
            return CurrentState.TryGetValue(key, out var value) ? value : 0;
        }
        
        public static bool InState(string key)
        {
            return CurrentState.ContainsKey(key) && CurrentState[key] > 0;
        }
        
        public static void ClearState(string key)
        {
            CurrentState.Remove(key);
        }
        
        public static void ClearAllStates()
        {
            CurrentState.Clear();
        }

        public static string GetCurrentWorldState()
        {
            var state = "";
            foreach (var (key, value) in CurrentState)
            {
                state += $"{key}: {value}\n";
            }
            
            return state;
        }
    }
}