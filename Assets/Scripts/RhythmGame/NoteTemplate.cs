using System;
using UnityEngine;

namespace RhythmGame
{
    [Serializable]
    public class NoteTemplate
    {
        public NoteType noteType;
        public Color singleNoteIcon;
        public Color longNoteIcon;
        public float position;
    }
}