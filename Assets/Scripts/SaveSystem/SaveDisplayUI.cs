using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public class SaveDisplayUI : MonoBehaviour
    {
        [SerializeField] private List<SaveDisplayButtonUI> saveDisplayButtons;

        private void OnEnable()
        {
            Display(SaveManager.Instance.Saves);
        }

        public void Display(List<Save> saves)
        {
            for (var i = 0; i < saveDisplayButtons.Count; i++)
            {
                saveDisplayButtons[i].Display(saves[i]);
            }
        }
        
        public void Save(int saveIndex)
        {
            SaveManager.Instance.Save(saveIndex);
            Display(SaveManager.Instance.Saves);
        }
        
        public void Load(int saveIndex)
        {
            SaveManager.Instance.Load(saveIndex);
        }
        
        public void Delete(int saveIndex)
        {
            SaveManager.Instance.DeleteSave(saveIndex);
            Display(SaveManager.Instance.Saves);
        }
    }
}