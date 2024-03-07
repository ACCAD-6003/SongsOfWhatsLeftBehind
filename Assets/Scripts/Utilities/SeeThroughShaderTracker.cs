using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughShaderTracker : MonoBehaviour
{
    public static Action<LayerMask, bool> SeeThroughChange;
    private static readonly int Size = Shader.PropertyToID("size");
    
    [SerializeField] Camera playerCamera;
    [SerializeField] private List<Material> material;
    [SerializeField] private LayerMask mask;

    private void Update()
    {
        var rayDirection = (playerCamera.transform.position - transform.position).normalized;
        
        if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, Mathf.Infinity, mask))
        {
            ToggleSeeThrough(true);
        }
        else
        {
            ToggleSeeThrough(false);
        }
    }
    
    private void ToggleSeeThrough(bool shouldSeeThrough)
    {
        material.ForEach(x => x.SetFloat(Size, shouldSeeThrough ? 1.5f : 0));
        SeeThroughChange?.Invoke(mask, shouldSeeThrough);
    }
}
