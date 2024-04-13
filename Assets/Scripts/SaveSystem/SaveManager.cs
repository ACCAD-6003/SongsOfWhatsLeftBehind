using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Dialogue_System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveSystem
{
    public class SaveManager : SerializedMonoBehaviour
    {
        private static SaveManager instance;
        public static SaveManager Instance 
        {
            get { if (instance == null) instance = FindObjectOfType<SaveManager>(); return instance; }
        }
        public List<Save> Saves => saves;

        readonly List<Save> saves = new () {new Save(), new Save(), new Save()};
        private Save savePrepared = new Save();
        private int currentSaveIndex;
        private bool isLoading;
        
        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SceneTools.onSceneTransitionStart += PrepareSave;
            ReadSavesFromFile();
        }
        
        private void PrepareSave(int sceneIndex)
        {
            savePrepared = new Save(WorldState.GetWorldState(), sceneIndex);
        }

        [Button]
        public void Save(int slotIndex)
        {
            saves[slotIndex] = new Save(savePrepared);
            Debug.Log("Saved to slot " + slotIndex);
        }
        
        [Button]
        public void Load(int saveIndex)
        {
            currentSaveIndex = saveIndex;
            WorldState.LoadWorldState(saves[currentSaveIndex].GetWorldState());
            StartCoroutine(SceneTools.TransitionToScene(saves[currentSaveIndex].sceneIndex));
            isLoading = true;
            Debug.Log("Loaded from slot " + saveIndex);
        }

        [Button]
        public void WriteSavesToFile()
        {
            var jsonData = JsonUtility.ToJson(new SavesJson(saves), true);
            Debug.Log(jsonData);
            if (Application.isEditor)
            {
                System.IO.File.WriteAllText(Application.dataPath + "/saves.json", jsonData);
            }
            else
            {
                System.IO.File.WriteAllText(Application.persistentDataPath + "/saves.json", jsonData);
            }
        }

        private void ReadSavesFromFile()
        {
            var path = Application.isEditor ? Application.dataPath : Application.persistentDataPath;
            if (!System.IO.File.Exists(path + "/saves.json")) return;
            
            var jsonData = System.IO.File.ReadAllText(path + "/saves.json");
            var loadedData = JsonUtility.FromJson<SavesJson>(jsonData);
            saves.Clear();
            saves.AddRange(loadedData.saves);
        }

        public void DeleteSave(int saveIndex)
        {
            saves[saveIndex] = null;
        }
        
        private void OnDestroy()
        {
            SceneTools.onSceneTransitionStart -= PrepareSave;
            if (Instance == this)
            {
                WriteSavesToFile();
            }
        }
    }
}