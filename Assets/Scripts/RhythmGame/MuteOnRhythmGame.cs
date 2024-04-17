using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    [RequireComponent(typeof(AudioSource))]
    public class MuteOnRhythmGame : MonoBehaviour
    {
        [SerializeField] RhythmGameManager rhythmGameManager;
        [SerializeField] float fadeDuration = 0.5f; 

        private AudioSource source;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            source.volume = 0;
            fadeCoroutine = StartCoroutine(FadeHandler(1));
            SceneTools.onSceneTransitionStart += Mute;
            if (rhythmGameManager == null) return;
            rhythmGameManager.OnSongStart += Mute;
            rhythmGameManager.OnSongEnd += Unmute;
        }

        private void Mute()
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeHandler(0));
        }
        
        private void Mute(SongData _)
        {
            Mute();
        }

        private void Mute(int _)
        {
            Mute();
        }
        
        private IEnumerator FadeHandler(float target)
        {
            var startTime = Time.time;
            var startVolume = source.volume;
            
            while (Time.time - startTime < fadeDuration)
            {
                source.volume = Mathf.Lerp(startVolume, target, (Time.time - startTime) / fadeDuration);
                yield return null;
            }
        }
        
        private void Unmute()
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeHandler(1));
        }
        
        private void OnDestroy()
        {
            SceneTools.onSceneTransitionStart -= Mute;
            if (rhythmGameManager == null) return;
            rhythmGameManager.OnSongStart -= Mute;
            rhythmGameManager.OnSongEnd -= Unmute;
        }
    }
}