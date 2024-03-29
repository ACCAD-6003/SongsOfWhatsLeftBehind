using TMPro;
using UnityEngine;

namespace QuestSystem
{
    public class QuestUIDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject display;
        [SerializeField] private TMP_Text questDescriptionText;

        public void Display(string description = "> No quest yet check back later!")
        {
            description ??= "> No quest yet check back later!";
            questDescriptionText.text = description;
            display.SetActive(true);
        }

        public void Hide()
        {
            display.SetActive(false);
        }
    }
}