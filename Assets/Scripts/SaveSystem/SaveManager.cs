using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Dialogue_System;
using UnityEngine;

namespace SaveSystem
{
    public class SaveManager : SingletonMonoBehavior<SaveManager>
    {
        private int currentSaveIndex;
        private Save CurrentSave => Saves[currentSaveIndex];

        public List<Save> Saves { get; private set; } = new(3);

        [Button]
        public void Save(int slotIndex)
        {
            Saves[slotIndex] = new Save(WorldState.GetWorldState(), Vector2.zero, SceneTools.CurrentSceneIndex);
            Debug.Log("Saved to slot " + slotIndex);
        }
        
        [Button]
        public void Load(int saveIndex)
        {
            currentSaveIndex = saveIndex;
            WorldState.LoadWorldState(CurrentSave.GetWorldState());
            StartCoroutine(SceneTools.TransitionToScene(CurrentSave.sceneIndex));
            Debug.Log("Loaded from slot " + saveIndex);
        }

        [Button]
        public void WriteSavesToFile()
        {
            var jsonData = JsonUtility.ToJson(Saves);
            if (Application.isEditor)
            {
                System.IO.File.WriteAllText(Application.dataPath + "/saves.json", jsonData);
            }
            else
            {
                System.IO.File.WriteAllText(Application.persistentDataPath + "/saves.json", jsonData);
            }
        }

        public void DeleteSave(int saveIndex)
        {
            Saves[saveIndex] = null;
        }
    }
}