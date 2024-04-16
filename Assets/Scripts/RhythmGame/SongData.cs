using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    [CreateAssetMenu(fileName = "New Song", menuName = "Rhythm Game/Song Data")]
    public class SongData : ScriptableObject
    {
        public AudioClip song;
        public AudioClip violinLayer;
        public List<PhraseData> phrases = new();
        public float bpm;
        public string SongName => name;
        public float SongLength => phrases[^1].startTime + phrases[^1].duration;
        public float speed = 1;
        public string dialogue;
    }
}