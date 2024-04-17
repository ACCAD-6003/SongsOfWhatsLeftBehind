using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace RhythmGame
{
    [RequireComponent(typeof(AudioSource))]
    public class RhythmGameMusicPlayer : MonoBehaviour
    {
        [SerializeField] private float volumeTransitionDuration = 0.2f;
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource violinTrack;
        [SerializeField] private RhythmGameManager rhythmGameManager;

        private Coroutine transition;

        private void Awake()
        {
            rhythmGameManager.OnHit += IncreaseVolume;
            rhythmGameManager.OnMiss += DecreaseVolume;
        }
        
        public void PlaySong(SongData songData, float startTime = 0)
        {
            audioSource.clip = songData.song;
            audioSource.time = startTime;
            audioSource.volume = PlayerPreferences.MusicVolume;
            violinTrack.clip = songData.violinLayer;
            violinTrack.time = startTime;
            violinTrack.volume = PlayerPreferences.ViolinVolume;
            StartCoroutine(PlayAfterDelay());
        }
        
        private IEnumerator PlayAfterDelay()
        {
            yield return new WaitForSeconds(PlayerPreferences.VisualDelay);
            audioSource.Play();
            if (violinTrack.clip != null) violinTrack.Play();
        }

        private void IncreaseVolume()
        {
            if (transition != null) StopCoroutine(transition);
            transition = StartCoroutine(TransitionVolume(PlayerPreferences.ViolinVolume));
        }

        private void DecreaseVolume()
        {
            if (transition != null) StopCoroutine(transition);
            transition = StartCoroutine(TransitionVolume(0f));
        }
        
        private IEnumerator TransitionVolume(float targetVolume)
        {
            var startVolume = violinTrack.volume;
            var startTime = Time.time;
            while (Time.time - startTime < volumeTransitionDuration)
            {
                violinTrack.volume = Mathf.Lerp(startVolume, targetVolume, (Time.time - startTime) / volumeTransitionDuration);
                yield return null;
            }
            violinTrack.volume = targetVolume;
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