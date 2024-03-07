using UnityEngine;

namespace UI.Dialogue_System
{
    public class TransitionAfterCutscene : MonoBehaviour
    {
        private void Start()
        {
            DialogueManager.OnDialogueEnded += HandleDialogueEnd;
        }

        private void HandleDialogueEnd()
        {
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.NextSceneExists ? SceneTools.NextSceneIndex : 0));
        }
        
        private void OnDestroy()
        {
            DialogueManager.OnDialogueEnded -= HandleDialogueEnd;
        }
    }
}