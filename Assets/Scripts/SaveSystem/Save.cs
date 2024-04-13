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

        public Save()
        {
            isEmpty = true;
            sceneIndex = 0;
            saveTime = DateTime.Now;
            worldState = new List<WorldStateEntry>();
        }
        
        public Save(Save save)
        {
            worldState = new List<WorldStateEntry>(save.worldState);
            saveTime = save.saveTime;
            sceneIndex = save.sceneIndex;
            isEmpty = save.isEmpty;
        }
        
        public Save(Dictionary<string, int> currentWorldState, int sceneIndex)
        {
            UpdateWorldState(currentWorldState);
            saveTime = DateTime.Now;
            this.sceneIndex = sceneIndex;
            isEmpty = false;
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