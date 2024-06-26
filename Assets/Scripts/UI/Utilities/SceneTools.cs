﻿using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class SceneTools
{
    // Returns next scene index
    public static Action<int> onSceneTransitionStart;
    public static bool transistioning = false;

    public static bool NextSceneExists => 
        NextSceneIndex < SceneManager.sceneCountInBuildSettings;

    public static int NextSceneIndex =>
         CurrentSceneIndex + 1;

    public static int CurrentSceneIndex =>
        SceneManager.GetActiveScene().buildIndex;


    public static IEnumerator TransitionToScene(int sceneIndex)
    {
        if (!transistioning)
        {
            transistioning = true;
            onSceneTransitionStart?.Invoke(sceneIndex);
            yield return FadeToBlackSystem.TryCueFadeInToBlack(1f);
            SceneManager.LoadScene(sceneIndex);
            transistioning = false;
        }
    }
}
