using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace SaveSystem
{
    [Serializable]
    public class Save
    {
        [SerializeField] private List<WorldStateEntry> worldState;
        public DateTime saveTime;
        public int sceneIndex;
        public bool isEmpty;
        public int day;

        public Save()
        {
            isEmpty = true;
            sceneIndex = 1;
            saveTime = DateTime.Now;
            worldState = new List<WorldStateEntry>();
            day = 0;
        }
        
        public Save(Save save)
        {
            worldState = new List<WorldStateEntry>(save.worldState);
            saveTime = save.saveTime;
            sceneIndex = save.sceneIndex;
            isEmpty = save.isEmpty;
            day = save.day + 1;
        }
        
        public Save(Dictionary<string, int> currentWorldState, int sceneIndex, int day)
        {
            UpdateWorldState(currentWorldState);
            saveTime = DateTime.Now;
            this.sceneIndex = sceneIndex;
            isEmpty = false;
            this.day = day;
        }
        
        private void UpdateWorldState(Dictionary<string, int> currentWorldState)
        {
            worldState = currentWorldState.Select(state => new WorldStateEntry(state.Key, state.Value)).ToList();
        }
        
        public Dictionary<string, int> GetWorldState()
        {
            return worldState.ToDictionary(entry => entry.entryName, entry => entry.entryValue);
        }
    }
}