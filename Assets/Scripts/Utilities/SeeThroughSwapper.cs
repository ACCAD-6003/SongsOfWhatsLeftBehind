using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SeeThroughSwapper : MonoBehaviour
{
    [SerializeField] private Material[] opaqueMaterials;
    [SerializeField] private Material[] seeThroughMaterials;

    private LayerMask mask;
    MeshRenderer meshRenderer;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mask = gameObject.layer;
        HandleSeeThroughChange(mask, false);
        Debug.Log(mask.value);
    }

    private void Start()
    {
        SeeThroughShaderTracker.SeeThroughChange += HandleSeeThroughChange;
    }
    
    private void HandleSeeThroughChange(LayerMask layerMask, bool shouldSeeThrough)
    {
        if (layerMask.value >> mask.value != 1) return;
        meshRenderer.materials = shouldSeeThrough ? seeThroughMaterials : seeThroughMaterials;
    }
    
    private void OnDestroy()
    {
        SeeThroughShaderTracker.SeeThroughChange -= HandleSeeThroughChange;
    }
}