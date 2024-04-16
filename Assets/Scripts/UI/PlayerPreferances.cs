using UnityEngine;

namespace UI
{
    public static class PlayerPreferences
    {
        public static float GlobalVolume
        {
            get => PlayerPrefs.GetFloat("GlobalVolume", 1);
            set => PlayerPrefs.SetFloat("GlobalVolume", Mathf.Clamp(value, 0, 1));
        }
        
        public static float MusicVolume
        {
            get => Mathf.Min(GlobalVolume, PlayerPrefs.GetFloat("MusicVolume", 1));
            set => PlayerPrefs.SetFloat("MusicVolume", Mathf.Clamp(value, 0, 1));
        }
        
        public static float ViolinVolume
        {
            get => Mathf.Min(GlobalVolume, PlayerPrefs.GetFloat("ViolinVolume", 1));
            set => PlayerPrefs.SetFloat("ViolinVolume", Mathf.Clamp(value, 0, 1));
        }

        public static float VisualDelay
        {
            get => PlayerPrefs.GetFloat("VisualDelay", 0);
            set => PlayerPrefs.SetFloat("VisualDelay", value);
        }
    }
}