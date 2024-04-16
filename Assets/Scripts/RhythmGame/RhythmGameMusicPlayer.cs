using UnityEngine;
using UnityEngine.Assertions;

namespace RhythmGame
{
    [RequireComponent(typeof(AudioSource))]
    public class RhythmGameMusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource violinTrack;
        [SerializeField] private RhythmGameManager rhythmGameManager;
        

        private void Awake()
        {
            rhythmGameManager.OnHit += IncreaseVolume;
            rhythmGameManager.OnMiss += DecreaseVolume;
        }
        
        public void PlaySong(SongData songData, float startTime = 0)
        {
            audioSource.clip = songData.song;
            audioSource.time = startTime;
            audioSource.Play();
            violinTrack.clip = songData.violinLayer;
            violinTrack.time = startTime;
            violinTrack.volume = 1f;
            if (violinTrack.clip != null) violinTrack.Play();
        }

        private void IncreaseVolume()
        {
            violinTrack.volume = 1f;
        }

        private void DecreaseVolume()
        {
            violinTrack.volume = 0f;
        }
        
        public void PlayErrorSound()
        {
            if (violinTrack.clip == null) audioSource.PlayOneShot(errorSound);
        }

        public void StopSong()
        {
            audioSource.Stop();
            violinTrack.Stop();
        }
    }
}