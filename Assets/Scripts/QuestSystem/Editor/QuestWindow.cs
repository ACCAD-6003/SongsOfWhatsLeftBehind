using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace QuestSystem.Editor
{
    public class QuestWindow : OdinEditorWindow
    {
        [SerializeField, TextArea(10, 40)] private string sectionToParse;

        [MenuItem("Tools/Quest System")]
        private static void OpenWindow()
        {
            GetWindow<QuestWindow>().Show();
        }

        [Button]
        public void ParseSection()
        {
            QuestConverter.ConvertToJson(sectionToParse);
        }
    }
    
    public static class QuestConverter
    {
        public static void ConvertToJson(string box)
        {
            Debug.Log("Converting to JSON: " + box);
            var questLines = box.Split("Quest: ").Where(x => !x.IsNullOrWhitespace());
            foreach (var questLine in questLines)
            {
                ConvertToQuestLine(questLine);
            }
        }
        
        private static void ConvertToQuestLine(string text)
        {
            var questLine = ScriptableObject.CreateInstance<QuestLine>();
            questLine.SetupQuestLine(text);
            SaveQuestLine(questLine);
        }
        
        /*
         * Will see if Resources/Quests has a QuestLine with the same name as the questLine being passed in
         * If it does, it will update the existing QuestLine with the new data and then mark it as dirty
         * If it does not, it will create a new QuestLine and save it to Resources/Quests
         */
        private static void SaveQuestLine(QuestLine questLine)
        {
            var filePath = $"Assets/Resources/Quests/{questLine.name}.asset";
            if (System.IO.File.Exists(filePath))
            {
                var file = AssetDatabase.LoadAssetAtPath(filePath, typeof(QuestLine)) as QuestLine;
                EditorUtility.CopySerialized(questLine, file);
                EditorUtility.SetDirty(file);
            }
            else
            {
                AssetDatabase.CreateAsset(questLine, filePath);
            }
        }
    }
}