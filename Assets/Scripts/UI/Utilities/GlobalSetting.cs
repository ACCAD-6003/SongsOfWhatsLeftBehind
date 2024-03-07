using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Setting", menuName = "SO/Global Setting")]
public class GlobalSetting : ScriptableObject
{
    public event Action<int> OnValueUpdated;

    [SerializeField] int min;
    [SerializeField] int max;
    [SerializeField] int value;

    public int Value 
    { 
        get => value; 
        set
        {
            this.value = Mathf.Clamp(value, min, max);
            OnValueUpdated?.Invoke(value);
        }
    }
}
