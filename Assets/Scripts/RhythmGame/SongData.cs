using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    [CreateAssetMenu(fileName = "New Song", menuName = "Rhythm Game/Song Data")]
    public class SongData : ScriptableObject
    {
        public AudioClip song;
        public List<PhraseData> phrases = new();
        public string SongName => name;
        public float SongLength => phrases[^1].startTime + phrases[^1].duration;
    }
}