using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SaveSystem
{
    public class SaveDisplayButtonUI : MonoBehaviour
    {
        private static Dictionary<int, string> sceneNames = new() {{1, "Graveyard"}, {2, "Town"}};

        [SerializeField] private TMP_Text day;
        [SerializeField] private TMP_Text sceneName;
        [SerializeField] private bool skewText;
        [SerializeField, ShowIf("skewText")] private float daySize = 80f;
        [SerializeField, ShowIf("skewText")] private float sceneSize = 60f;

        public void Display(Save save)
        {
            if (day == null)
            {
                if (skewText)
                {
                    sceneName.text = save.isEmpty
                        ? "Empty"
                        : $"<size={daySize}>Day {ConvertNumToText(save.day)}</size>\n" +
                          $"<size={sceneSize}>{sceneNames[save.sceneIndex]}</size>";
                }
                else
                {
                    sceneName.text = save.isEmpty
                        ? "Empty"
                        : $"Day {ConvertNumToText(save.day)}\n{sceneNames[save.sceneIndex]}";
                }
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