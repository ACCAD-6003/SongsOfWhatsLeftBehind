﻿using System;
using Controller;
using Sirenix.OdinInspector;
using UI.Dialogue_System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.InteractionSystem
{
    public class Interactable : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";
        
        [SerializeField] private UnityEvent onInRange;
        [SerializeField] private UnityEvent onOutOfRange;
        [SerializeField] private bool goToNextScene;
        [SerializeField, HideIf("goToNextScene")] private SOConversationData conversation;
        [SerializeField, HideIf("goToNextScene")] private UnityEvent onTriggerDialogue;

        private int playersInRange;
        
        private static bool HasTag(GameObject objectToCheck, string tagToCheck)
        {
            return objectToCheck.gameObject.CompareTag(tagToCheck);
        } 
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!HasTag(other.gameObject, PLAYER_TAG)) return;

            onInRange.Invoke();
            UIController.OnInteract += goToNextScene switch
            {
                false => TriggerDialogue,
                true => GoToNextScene
            };
        }
        
        private void GoToNextScene()
        {
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.NextSceneExists ? SceneTools.NextSceneIndex : 0));
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!HasTag(other.gameObject, PLAYER_TAG)) return;

            onOutOfRange.Invoke();
            UIController.OnInteract -= goToNextScene switch
            {
                false => TriggerDialogue,
                true => GoToNextScene
            };
        }

        private void TriggerDialogue()
        {
            onTriggerDialogue.Invoke();
            DialogueManager.Instance.StartDialogue(conversation);
        }

        private void OnDestroy()
        {
            UIController.OnInteract -= goToNextScene switch
            {
                false => TriggerDialogue,
                true => GoToNextScene
            };
        }
    }
}