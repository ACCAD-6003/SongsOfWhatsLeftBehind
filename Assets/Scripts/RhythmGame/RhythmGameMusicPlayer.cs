using System;
using System.Collections;
using UI;
using UI.Dialogue_System;
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
        [SerializeField] private AudioSource vocalTrack;
        [SerializeField] private RhythmGameManager rhythmGameManager;

        private Coroutine transition;
        private static bool InGarretSong => WorldState.InState("ShowGarretInSong");

        private void Awake()
        {
            rhythmGameManager.OnHit += IncreaseVolume;
            rhythmGameManager.OnMiss += DecreaseVolume;
        }
        
        public void PlaySong(SongData songData, float startTime = 0)
        {
            SetupAudio(audioSource, songData.song, startTime, AudioPreferences.MusicVolume);
            if (songData.violinLayer != null) 
                SetupAudio(violinTrack, songData.violinLayer, startTime, AudioPreferences.ViolinVolume);
            if (InGarretSong && songData.vocalLayer != null) 
                SetupAudio(vocalTrack, songData.vocalLayer, startTime, AudioPreferences.ViolinVolume);
        }

        private void SetupAudio(AudioSource source, AudioClip clip, float startTime, float volume)
        {
            source.clip = clip;
            source.time = startTime;
            source.volume = volume;
            source.Play();
        }

        private void IncreaseVolume()
        {
            if (transition != null) StopCoroutine(transition);
            transition = StartCoroutine(TransitionVolume(AudioPreferences.ViolinVolume));
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
                vocalTrack.volume = violinTrack.volume;
                yield return null;
            }
            violinTrack.volume = targetVolume;
            vocalTrack.volume = targetVolume;
        }
        
        public void PlayErrorSound()
        {
            if (violinTrack.clip == null) audioSource.PlayOneShot(errorSound);
        }

        public void StopSong()
        {
            audioSource.Stop();
            violinTrack.Stop();
            vocalTrack.Stop();
        }
    }
}