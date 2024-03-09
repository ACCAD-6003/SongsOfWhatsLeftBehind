using System;
using System.Collections.Generic;

namespace RhythmGame
{
    [Serializable]
    public class NoteData
    {
        public float offset;
        public float noteLength;
        public NoteStyle style;
        public List<NoteType> notes = new();
    }
}