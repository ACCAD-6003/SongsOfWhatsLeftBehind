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
        public string saveName;
        public List<WorldStateEntry> worldState;
        public Vector2 playerPosition;
        public DateTime saveTime;
        public int sceneIndex;
        
        public Save(Dictionary<string, int> currentWorldState, Vector2 playerPosition, int sceneIndex)
        {
            UpdateWorldState(currentWorldState);
            saveTime = DateTime.Now;
            this.playerPosition = playerPosition;
            this.sceneIndex = sceneIndex;
        }
        
        private void UpdateWorldState(Dictionary<string, int> currentWorldState)
        {
            worldState.Clear();
            worldState = currentWorldState.Select(state => new WorldStateEntry(state.Key, state.Value)).ToList();
        }
        
        public Dictionary<string, int> GetWorldState()
        {
            return worldState.ToDictionary(entry => entry.entryName, entry => entry.entryValue);
        }

        [Serializable]
        public class WorldStateEntry
        {
            public string entryName;
            public int entryValue;
            
            public WorldStateEntry(string entryName, int entryValue)
            {
                this.entryName = entryName;
                this.entryValue = entryValue;
            }
        }
    }
}