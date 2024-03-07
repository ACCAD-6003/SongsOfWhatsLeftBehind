using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;

public class AudioControls : SerializedMonoBehaviour
{
    [SerializeField] new AudioSource audio;
    [SerializeField] GlobalSetting volume;

    private void OnEnable()
    {
        volume.OnValueUpdated += UpdateVolume;
        UpdateVolume(volume.Value);
    }

    private void UpdateVolume(int newVolume)
    {
        audio.volume = newVolume / 100f;
    }

    private void OnDisable()
    {
        volume.OnValueUpdated -= UpdateVolume;
    }
}
