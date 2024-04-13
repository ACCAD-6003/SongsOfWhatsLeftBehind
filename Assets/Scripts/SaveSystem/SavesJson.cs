using System;
using System.Collections.Generic;

namespace SaveSystem
{
    [Serializable]
    internal class SavesJson
    {
        public List<Save> saves;
        public SavesJson(List<Save> saves)
        {
            this.saves = saves;
        }
    }
}