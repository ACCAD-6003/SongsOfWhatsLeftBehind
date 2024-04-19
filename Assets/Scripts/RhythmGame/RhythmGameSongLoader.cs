using System;
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
            var pieces = songEventName.Split("-");
            if (pieces.Length < 2)
            {
                Debug.LogWarning("Invalid song event name format. Expected format: PlaySong-[songName]-[nextDialogue] or" +
                                 "PlaySong-[songName]");
                return;
            }
            var songName = pieces[1].Trim();
            var nextDialogue = pieces.Length > 2 ? pieces[2].Trim() : $"{songName}SongEnd";
            
            if (songMappings.TryGetValue(songName, out var mapping))
            {
                manager.PlaySong(mapping, nextDialogue);
            }
        }
    }
}