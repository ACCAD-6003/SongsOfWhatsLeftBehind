using System;
using UnityEngine;

namespace UI
{
    public static class AudioPreferences
    {
        public enum VolumeType { Global, Music, Violin }
        public static Action<VolumeType, float> OnVolumeChanged;
        
        public static float GlobalVolume
        {
            get => PlayerPrefs.GetFloat("GlobalVolume", 1);
            set
            {
                PlayerPrefs.SetFloat("GlobalVolume", Mathf.Clamp(value, 0, 1)); 
                OnVolumeChanged?.Invoke(VolumeType.Global, GlobalVolume);
                OnVolumeChanged?.Invoke(VolumeType.Music, MusicVolume);
                OnVolumeChanged?.Invoke(VolumeType.Violin, ViolinVolume);
            }
        }

        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume", 1) * GlobalVolume;
            set
            {
                PlayerPrefs.SetFloat("MusicVolume", Mathf.Clamp(value, 0, 1)); 
                OnVolumeChanged?.Invoke(VolumeType.Music, MusicVolume);
            }
        }
        
        public static float ViolinVolume
        {
            get => PlayerPrefs.GetFloat("ViolinVolume", 1) * GlobalVolume;
            set
            {
                PlayerPrefs.SetFloat("ViolinVolume", Mathf.Clamp(value, 0, 1)); 
                OnVolumeChanged?.Invoke(VolumeType.Violin, ViolinVolume);
            }
        }

        public static float GetVolume(VolumeType volumeType)
        {
            return volumeType switch
            {
                VolumeType.Global => GlobalVolume,
                VolumeType.Music => MusicVolume,
                VolumeType.Violin => ViolinVolume,
                _ => throw new ArgumentOutOfRangeException(nameof(volumeType), volumeType, null)
            };
        }
    }
}