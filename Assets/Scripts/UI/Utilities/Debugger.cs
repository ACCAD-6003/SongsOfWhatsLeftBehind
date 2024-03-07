using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Utilities
{
    public class Debugger : MonoBehaviour
    {
        [SerializeField] private List<DebugEvent> events;

        [Button(ButtonStyle.Box)]
        private void TriggerEvent(string nameOfEvent)
        {
            events.Find(e => e.eventName.Equals(nameOfEvent))?.TriggerEvent();
        }
        
        
        [Serializable]
        private class DebugEvent
        {
            public string eventName;
            public UnityEvent eventToTrigger;
            
            public void TriggerEvent()
            {
                eventToTrigger.Invoke();
            }
        }
    }
}