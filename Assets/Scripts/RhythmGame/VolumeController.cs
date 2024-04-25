using System;
using UI;
using UnityEngine;

namespace RhythmGame
{
    [RequireComponent(typeof(AudioSource))]
    public class VolumeController : MonoBehaviour
    {
        [SerializeField] private AudioPreferences.VolumeType volumeType;
        
        private AudioSource audioSource;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = AudioPreferences.GetVolume(volumeType);
            AudioPreferences.OnVolumeChanged += OnVolumeChanged;
        }

        private void OnEnable()
        {
            audioSource.volume = AudioPreferences.GetVolume(volumeType);
        }

        private void OnDestroy()
        {
            AudioPreferences.OnVolumeChanged -= OnVolumeChanged;
        }
        
        private void OnVolumeChanged(AudioPreferences.VolumeType type, float volume)
        {
            if (type == volumeType) audioSource.volume = volume;
        }
    }
}