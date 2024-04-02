using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Dialogue_System
{
    public class DialogueEventReceiver : MonoBehaviour
    {
        [SerializeField] private string eventToReceive;
        [SerializeField] private UnityEvent onEventReceived;

        private void OnEnable()
        {
            DialogueManager.OnEventTriggered += OnEventTriggered;
        }
        
        private void OnEventTriggered(string eventToReceive)
        {
            if (eventToReceive != this.eventToReceive) return;
            TriggerEvent();
        }

        [Button]
        private void TriggerEvent()
        {
            onEventReceived?.Invoke();
        }
        
        private void OnDisable()
        {
            DialogueManager.OnEventTriggered -= OnEventTriggered;
        }
    }
}