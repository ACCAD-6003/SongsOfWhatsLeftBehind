using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue_System
{
    public class EndingsDisplayHandler : MonoBehaviour
    {
        private static int endingsUnlocked = 0;
        [SerializeField] private List<EndingNode> endingImages;

        private bool currentlyInQuest;

        private void OnEnable()
        {
            LoadEndings();
            DisplayEndingImages();
            SaveEndings();
        }
        
        private void UnlockEnding(int endingIndex)
        {
            endingsUnlocked |= 1 << endingIndex;
            DisplayEndingImages();
        }

        private void LoadEndings()
        {
            for (int i = 0; i < endingImages.Count; i++)
            {
                if (WorldState.InState(endingImages[i].unlock)) UnlockEnding(i);
            }
            
            endingsUnlocked |= PlayerPrefs.GetInt("Endings", 0);
        }
        
        private void SaveEndings()
        {
            PlayerPrefs.SetInt("Endings", endingsUnlocked);
        }
        
        private void DisplayEndingImages()
        {
            for (var i = 0; i < endingImages.Count; i++)
            {
                endingImages[i].image.enabled = (endingsUnlocked & 1 << i) != 0;
            }
        }
        
        [Serializable]
        private class EndingNode
        {
            public string unlock;
            public Image image;
        }
    }
}