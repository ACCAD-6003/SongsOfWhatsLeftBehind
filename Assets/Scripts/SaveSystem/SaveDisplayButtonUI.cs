using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SaveSystem
{
    public class SaveDisplayButtonUI : MonoBehaviour
    {
        private static Dictionary<int, string> sceneNames = new() {{1, "Graveyard"}, {2, "Town"}};

        [SerializeField] private TMP_Text day;
        [SerializeField] private TMP_Text sceneName;

        public void Display(Save save)
        {
            if (day == null)
            {
                sceneName.text = save.isEmpty
                    ? "Empty"
                    : $"Day {ConvertNumToText(save.day)}\n{sceneNames[save.sceneIndex]}";
            }
            else
            {
                day.text = save.isEmpty ? "Empty" : $"Day {ConvertNumToText(save.day)}";
                sceneName.text = save.isEmpty ? "Empty" : sceneNames[save.sceneIndex];
                sceneName.enabled = !save.isEmpty;
            }
        }

        private static string ConvertNumToText(int num)
        {
            return num switch
            {
                1 => "One",
                2 => "Two",
                3 => "Three",
                4 => "Four",
                5 => "Five",
                6 => "Six",
                7 => "Seven",
                8 => "Eight",
                9 => "Nine",
                10 => "Ten",
                _ => "Unknown"
            };
        }
    }
}