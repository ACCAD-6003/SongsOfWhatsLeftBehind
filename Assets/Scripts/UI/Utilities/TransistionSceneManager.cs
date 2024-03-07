using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class TransistionSceneManager : MonoBehaviour
{
    [SerializeField] float delayBeforeTransition;
    [SerializeField] private bool autoTransition;

    private void Start()
    {
        if(autoTransition)
            StartCoroutine(HandleTransitions());
    }

    [Button]
    public void TransitionToNextScene()
    {
        StartCoroutine(HandleTransitions());
    }
    
    [Button]
    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator HandleTransitions()
    {
        yield return new WaitForSeconds(delayBeforeTransition);

        var nextScene = SceneTools.NextSceneExists ? SceneTools.NextSceneIndex : 0;

        StartCoroutine(SceneTools.TransitionToScene(nextScene));

    }
}
