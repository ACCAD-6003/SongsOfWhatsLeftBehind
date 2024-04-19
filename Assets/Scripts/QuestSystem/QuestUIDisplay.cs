using TMPro;
using UnityEngine;

namespace QuestSystem
{
    public class QuestUIDisplay : MonoBehaviour
    {
        private const string NO_QUEST_TEXT = "No quest yet, check back later!";
        [SerializeField] private GameObject display;
        [SerializeField] private TMP_Text questDescriptionText;

        public void Display(string description = NO_QUEST_TEXT)
        {
            description ??= NO_QUEST_TEXT;
            questDescriptionText.text = description;
            display.SetActive(true);
        }

        public void Hide()
        {
            display.SetActive(false);
        }
    }
}