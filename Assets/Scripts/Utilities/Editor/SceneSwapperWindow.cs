using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities.Editor
{
    public class ColliderDestroyer : OdinEditorWindow
    {
        [Button]
        private static void DestroyMesh(GameObject go)
        {
            foreach (Transform child in go.transform)
            {
                if (child.TryGetComponent<Collider>(out var collider))
                {
                    DestroyImmediate(collider, true);
                }

                if (child != go.transform)
                {
                    DestroyMesh(child.gameObject);
                }
            }
        }
        
        [MenuItem("Tools/Destroy Colliders")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ColliderDestroyer));
        }
    }
    
    /// <summary>
    /// Provides a custom unity editor window to facilitate quick transitioning between scenes.
    /// Included Options:
    ///     - Ability to click button to jump to that scene
    ///     - A List of all scenes located in Assets/Scenes
    ///     - Ability to refresh the scene list
    /// </summary>
    public class SceneSwapperWindow : EditorWindow
    {
        private const string SCENE_FOLDER = "Assets/Scenes/";
        private const string FILE_EXTENSION = ".unity";

        private List<string> currentScenes = new();
        
        private static void SwapToScene(string sceneName)
        {
            if (Application.isPlaying)
            {
                SceneManager.LoadScene(NameWithoutExtension(sceneName));
            }
            else if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(SCENE_FOLDER + sceneName, OpenSceneMode.Single);
            }
        }

        [MenuItem("Tools/Scene Swapper")]
        public static void ShowWindow()
        {
            var window = (SceneSwapperWindow)GetWindow(typeof(SceneSwapperWindow), false, "Scene Swapper");
            window.UpdateSceneList();
        }

        private void OnGUI()
        {
            foreach (var scene in currentScenes)
            {
                if (GUILayout.Button(NameWithoutExtension(scene)))
                {
                    SwapToScene(scene);
                }
            }
            
            if (GUILayout.Button("Update Scene List"))
            {
                UpdateSceneList();
            }
        }

        private void UpdateSceneList()
        {
            var info = new DirectoryInfo("Assets/Scenes");
            var fileInfo = info.GetFiles();
            currentScenes.Clear();
            foreach (var file in fileInfo.Where(x => x.Name.EndsWith(FILE_EXTENSION)))
            {
                currentScenes.Add(file.Name);
            }
        }

        private static string NameWithoutExtension(string name)
        {
            return name.Remove(name.Length - FILE_EXTENSION.Length);
        }
    }
}