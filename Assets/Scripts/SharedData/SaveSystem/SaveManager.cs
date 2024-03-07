/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2022, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SharedData.SaveSystem
{
    /// <summary>
    /// Handles the saving and loading of save files within the application.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private const string SAVE_FOLDER_NAME = "SaveData";
        private const string DEFAULT_SAVE_FILE_NAME = "DefaultSave";
        private const string EXTENSION = ".json";

#if UNITY_EDITOR
        private static string SaveFolderPath => $"{Application.dataPath}//{SAVE_FOLDER_NAME}//";
        private static string DefaultPath => Application.dataPath + "/Resources/" + DEFAULT_SAVE_FILE_NAME + EXTENSION;
#else
        private static string SaveFolderPath => $"{Application.persistentDataPath}//{SAVE_FOLDER_NAME}//";
#endif
        private static string FilePathTo(string fileName)
        {
            return SaveFolderPath + fileName + EXTENSION;
        }

        [SerializeField] private SaveGroup saveGroup;
        [SerializeField] private SharedEvent onReset;
        [SerializeField] private SharedEvent onSave;
        [SerializeField] private string saveFileName = "";

        private void Awake()
        {
            onReset.Value += ResetToDefault;
            onSave.Value += SaveGame;
            SetupSaveFolder();
        }

        private static void SetupSaveFolder()
        {
            if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);
        }

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneChanged;
        }

        [ContextMenu("Reset to Default")]
        public void ResetToDefault()
        {
            var loadedFile = Resources.Load<TextAsset>(DEFAULT_SAVE_FILE_NAME);
            if (loadedFile != null)
            {
                Debug.Log("Resetting Data");
                if (!saveGroup.ValidateData(JsonUtility.FromJson<SaveData>(loadedFile.text)))
                {
                    Debug.Log("The default data has not been updated to new data that needs saving");
                    return;
                }

                saveGroup.LoadData(JsonUtility.FromJson<SaveData>(loadedFile.text));
                Resources.UnloadAsset(loadedFile);
                SaveGame();
                Debug.Log("Data Reset");
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Set Default to Current Save")]
        private void SetDefaultToCurrentSave()
        {
            var savesAsJson = JsonUtility.ToJson(saveGroup.SaveData);
            File.WriteAllText(DefaultPath, savesAsJson);
        }
#endif

        public void SwitchToSave(string newSaveName)
        {
            SaveGame();
            saveFileName = newSaveName;
            LoadGame();
        }

        [ContextMenu("Save Game")]
        public void SaveGame()
        {
            SaveGame(saveFileName);
        }

        public void SaveGame(string fileName)
        {
            if (fileName == "") return;

            if (saveFileName != fileName)
            {
                SaveGame(saveFileName);
                saveFileName = fileName;
            }

            var filePath = FilePathTo(fileName);
            var savesAsJson = JsonUtility.ToJson(saveGroup.SaveData);
            File.WriteAllText(filePath, savesAsJson);
        }

        [ContextMenu("Load Game")]
        public void LoadGame()
        {
            LoadGame(saveFileName);
        }

        public void LoadGame(string fileName)
        {
            var filePath = FilePathTo(fileName);
            if (File.Exists(filePath))
            {
                Debug.Log("Loading Data");
                var loadedFile = JsonUtility.FromJson<SaveData>(File.ReadAllText(filePath));
                if (!saveGroup.ValidateData(loadedFile))
                {
                    Debug.Log("Restoring Data");
                    ResetToDefault();
                    loadedFile = JsonUtility.FromJson<SaveData>(File.ReadAllText(filePath));
                }

                saveGroup.LoadData(loadedFile);
                Debug.Log("Data Loaded");
            }
        }

        public static void DeleteSave(string fileName)
        {
            var filePath = FilePathTo(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
#if UNITY_EDITOR
                File.Delete(filePath + ".meta");
#endif
            }
        }

        private void OnSceneChanged(Scene scene, LoadSceneMode __)
        {
            SaveGame();
            if (scene.name == "Setup Screen") saveFileName = "";
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void OnDestroy()
        {
            onReset.Value -= ResetToDefault;
            onSave.Value -= SaveGame;
            SceneManager.sceneLoaded -= OnSceneChanged;
        }
    }
}