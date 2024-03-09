using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RhythmGame
{
    [Serializable]
    public class PhraseData
    {
        public string phraseName;
        public float startTime;
        public List<NoteData> notes = new();
        public float duration => notes[^1].offset;
    }
}