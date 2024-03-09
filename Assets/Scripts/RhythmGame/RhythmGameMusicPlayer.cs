using UnityEngine;

namespace RhythmGame
{
    [RequireComponent(typeof(AudioSource))]
    public class RhythmGameMusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip errorSound;
        
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        public void PlaySong(SongData songData, float startTime = 0)
        {
            audioSource.clip = songData.song;
            audioSource.time = startTime;
            audioSource.Play();
        }
        
        public void PlayErrorSound()
        {
            audioSource.PlayOneShot(errorSound);
        }

        public void StopSong()
        {
            audioSource.Stop();
        }
    }
}