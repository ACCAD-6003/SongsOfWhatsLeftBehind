using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventButton : UIButton
{
    [SerializeField] UnityEvent eventToTrigger;
    
    public override void Use()
    {
        eventToTrigger?.Invoke();
    }
}
