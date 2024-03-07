using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] ButtonGroup menuButtons;
    [SerializeField] int firstLevelIndex;

    private void Start()
    {
        UIController.Instance.SwapToUI();
        menuButtons.EnableButtons();
    }

    public void StartGame()
    {
        UIController.Instance.SwapToGameplay();
        StartCoroutine(SceneTools.TransitionToScene(firstLevelIndex));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
