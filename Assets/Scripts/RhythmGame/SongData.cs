using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    [CreateAssetMenu(fileName = "New Song", menuName = "Rhythm Game/Song Data")]
    public class SongData : ScriptableObject
    {
        public AudioClip song;
        public int maxScore;
        public AudioClip violinLayer;
        public AudioClip vocalLayer;
        public List<PhraseData> phrases = new();
        public float bpm;
        public string SongName => name;
        public float speed = 1;
    }
}