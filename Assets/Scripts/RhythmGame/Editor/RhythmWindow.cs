using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace RhythmGame.Editor
{
    public class RhythmWindow : OdinEditorWindow
    {
        [SerializeField, TextArea(10, 40)] string box;

        [MenuItem("Tools/Rhythm Converter")]
        private static void OpenWindow()
        {
            GetWindow<RhythmWindow>().Show();
        }

        [Button]
        public void TryToConvert()
        {
            RhythmConverter.ConvertToJson(box);
        }
    }
}
