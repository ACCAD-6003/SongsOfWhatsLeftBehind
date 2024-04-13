using System;

namespace SaveSystem
{
    [Serializable]
    internal class WorldStateEntry
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