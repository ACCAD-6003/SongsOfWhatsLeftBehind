using System;
using UnityEngine;

namespace RhythmGame
{
    [RequireComponent(typeof(AudioSource))]
    public class MuteOnRhythmGame : MonoBehaviour
    {
        [SerializeField] RhythmGameManager rhythmGameManager;

        private AudioSource source;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            rhythmGameManager.OnSongStart += Mute;
            rhythmGameManager.OnSongEnd += Unmute;
        }
        
        private void Mute(SongData _)
        {
            source.volume = 0;
        }
        
        private void Unmute()
        {
            source.volume = 1;
        }
        
        private void OnDestroy()
        {
            rhythmGameManager.OnSongStart -= Mute;
            rhythmGameManager.OnSongEnd -= Unmute;
        }
    }
}