using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue_System
{
    public class EndingsDisplayHandler : MonoBehaviour
    {
        private static int endingsUnlocked = 0;
        [SerializeField] private List<Image> endingImages;

        public static void UnlockEnding(int quest)
        {
            endingsUnlocked |= 1 << quest;
        }

        private void OnEnable()
        {
            LoadEndings();
            DisplayEndingImages();
            SaveEndings();
        }
        
        private void LoadEndings()
        {
            endingsUnlocked = endingsUnlocked | PlayerPrefs.GetInt("Endings", 0);
        }
        
        private void SaveEndings()
        {
            PlayerPrefs.SetInt("Endings", endingsUnlocked);
        }
        
        private void DisplayEndingImages()
        {
            for (var i = 0; i < endingImages.Count; i++)
            {
                endingImages[i].enabled = (endingsUnlocked & 1 << i) != 0;
            }
        }
    }
}