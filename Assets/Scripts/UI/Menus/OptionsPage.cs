using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionsPage : MonoBehaviour
    {
        [SerializeField] private Slider globalVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider violinVolumeSlider;
        
        private void Start()
        {
            globalVolumeSlider.value = AudioPreferences.GlobalVolume;
            globalVolumeSlider.onValueChanged.AddListener(SetGlobalVolume);
            musicVolumeSlider.value = AudioPreferences.MusicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            violinVolumeSlider.value = AudioPreferences.ViolinVolume;
            violinVolumeSlider.onValueChanged.AddListener(SetViolinVolume);
        }
        
        private void SetGlobalVolume(float value)
        {
            AudioPreferences.GlobalVolume = value;
        }
        
        private void SetMusicVolume(float value)
        {
            AudioPreferences.MusicVolume = value;
        }

        private void SetViolinVolume(float value)
        {
            AudioPreferences.ViolinVolume = value;
        }
        
        private void OnDestroy()
        {
            globalVolumeSlider.onValueChanged.RemoveListener(SetGlobalVolume);
            musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
            violinVolumeSlider.onValueChanged.RemoveListener(SetViolinVolume);
        }
        
        private void OnDisable()
        {
            PlayerPrefs.Save();
        }
        
        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }
    }
}