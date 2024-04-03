using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    [RequireComponent(typeof(RhythmGameManager))]
    public class RhythmGameSongLoader : SerializedMonoBehaviour
    {
        [SerializeField] Dictionary<string, SongData> songMappings;

        RhythmGameManager manager;

        private void Awake()
        {
            manager = GetComponent<RhythmGameManager>();
        }

        public void PlaySong(string songEventName)
        {
            Debug.Assert(songMappings.ContainsKey(songEventName), "Failed to find song " + songEventName);
            manager.PlaySong(songMappings[songEventName]);
        }
    }
}