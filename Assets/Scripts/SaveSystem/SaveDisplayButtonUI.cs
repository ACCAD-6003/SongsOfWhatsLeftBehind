using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SaveSystem
{
    public class SaveDisplayButtonUI : MonoBehaviour
    {
        private static Dictionary<int, string> sceneNames = new() {{1, "Graveyard"}, {2, "Town"}};

        //[SerializeField] private TMP_Text saveTime;
        //[SerializeField] private TMP_Text saveDate;
        [SerializeField] private TMP_Text sceneName;

        public void Display(Save save)
        {
            //saveTime.text = save.saveTime.ToShortTimeString();
            //saveDate.text = save.saveTime.ToShortDateString();
            sceneName.text = save.isEmpty ? "Empty" : sceneNames[save.sceneIndex];
            
            //saveTime.gameObject.SetActive(!save.isEmpty);
            //saveDate.gameObject.SetActive(!save.isEmpty);
        }
    }
}