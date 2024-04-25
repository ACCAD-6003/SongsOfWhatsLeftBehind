using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Controller;
using TMPro;
using UI.Dialogue_System;
using UI.Menus;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RhythmGame
{
    public class TutorialPopup : MonoBehaviour
    {
        [SerializeField] private GameObject display;
        [SerializeField] private TMP_Text tutorialText;
        [SerializeField] private Image tutorialImage;
        [SerializeField] private PopupGroup movementTutorial;
        [SerializeField] private Image advanceButton;
        [SerializeField] private Sprite closeButton;
        [SerializeField] private Sprite nextButton;
        [SerializeField] private ButtonGroup buttonGroup;
        [SerializeField] private Animator animator;

        int currentIndex = 0;

        private void Awake()
        {
            display.SetActive(false);
        }

        public void OpenTutorial()
        {
            UIController.Instance.SwapToUI();
            display.SetActive(true);
            OpenPopup(0);
        }
        
        private void OpenPopup(int index)
        {
            var popup = movementTutorial.popups[index];
            var text = popup.tutorialText;
            var pattern = "\\[Button:(.*?)\\]";
            
            var match = Regex.Match(text, pattern);
            var keysUsed = new List<string>();
            while (match.Success)
            {
                keysUsed.Add(match.Groups[1].Value);
                match = match.NextMatch();
            }
            
            foreach (var keyName in keysUsed.Distinct())
            {
                var button = UIController.Instance.GetLongKey(keyName);
                var stringToMatch = $"[Button:{keyName}]";
                if (button == "W") button = "Arrow Keys or WASD"; 
                
                text = text.Replace(stringToMatch, button);
            }
            tutorialText.text = text;
            animator.enabled = popup.animation != null;
            animator.runtimeAnimatorController = popup.animation;
            tutorialImage.sprite = popup.image;
            currentIndex = index;
            
            UpdateButtons();
        }
        
        public void AdvancePopup()
        {
            if (currentIndex + 1 < movementTutorial.popups.Count)
            {
                OpenPopup(currentIndex + 1);
            }
            else
            {
                ClosePopup();
            }
        }
        
        public void BackupPopup()
        {
            if (currentIndex - 1 >= 0)
            {
                OpenPopup(currentIndex - 1);
            }
        }
        
        private void UpdateButtons()
        {
            advanceButton.sprite = currentIndex + 1 < movementTutorial.popups.Count ? nextButton : closeButton;
        }
        
        public void ClosePopup()
        {
            display.SetActive(false);
            UIController.Instance.SwapToGameplay();
        }
        
    }

    [Serializable]
    public class PopupData
    {
        [TextArea(3, 5)] public string tutorialText;
        public Sprite image;
        public RuntimeAnimatorController animation;
    }
}