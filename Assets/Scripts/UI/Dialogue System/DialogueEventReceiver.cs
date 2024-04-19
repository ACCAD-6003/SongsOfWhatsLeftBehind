using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Dialogue_System
{
    public class DialogueEventReceiver : MonoBehaviour
    {
        [SerializeField] private string eventToReceive;
        [SerializeField] private UnityEvent<string> onEventReceived;
        [SerializeField] private EventFormat eventFormat = EventFormat.Full;
        
        private enum EventFormat {Prefix, Suffix, Full, All}
        
        private void OnEnable()
        {
            DialogueManager.OnEventTriggered += OnEventTriggered;
        }
        
        private void OnEventTriggered(string eventToReceive)
        {
            Dictionary<EventFormat, Predicate<string>> eventFormatPredicates = new()
            {
                {EventFormat.All, _ => true},
                {EventFormat.Prefix, x => x.StartsWith(this.eventToReceive, StringComparison.CurrentCultureIgnoreCase)},
                {EventFormat.Suffix, x => x.EndsWith(this.eventToReceive, StringComparison.CurrentCultureIgnoreCase)},
                {EventFormat.Full, x => x.Equals(this.eventToReceive, StringComparison.CurrentCultureIgnoreCase)}
            };
            
            if (eventFormatPredicates[eventFormat](eventToReceive))
            {
                TriggerEvent(eventToReceive);
            }
        }

        [Button]
        private void TriggerEvent(string eventLabel)
        {
            onEventReceived?.Invoke(eventLabel);
        }
        
        private void OnDisable()
        {
            DialogueManager.OnEventTriggered -= OnEventTriggered;
        }
    }
}