using System;
using UnityEngine;

namespace RhythmGame
{
    [Serializable]
    public class NoteTemplate
    {
        public NoteType noteType;
        public Sprite noteIcon;
        public float position;
        public float marginX;
        public float endPointX;
    }
}