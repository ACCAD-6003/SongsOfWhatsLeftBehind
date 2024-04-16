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
            globalVolumeSlider.value = PlayerPreferences.GlobalVolume;
            globalVolumeSlider.onValueChanged.AddListener(SetGlobalVolume);
            musicVolumeSlider.value = PlayerPreferences.MusicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            violinVolumeSlider.value = PlayerPreferences.ViolinVolume;
            violinVolumeSlider.onValueChanged.AddListener(SetViolinVolume);
        }
        
        private void SetGlobalVolume(float value)
        {
            PlayerPreferences.GlobalVolume = value;
        }
        
        private void SetMusicVolume(float value)
        {
            PlayerPreferences.MusicVolume = value;
        }

        private void SetViolinVolume(float value)
        {
            PlayerPreferences.ViolinVolume = value;
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