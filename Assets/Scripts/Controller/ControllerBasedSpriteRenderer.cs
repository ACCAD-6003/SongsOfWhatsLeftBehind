using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class ControllerBasedSpriteRenderer : MonoBehaviour
{
    [SerializeField] private Sprite controllerSprite;
    [SerializeField] private Sprite keyboardSprite;
    
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        spriteRenderer.sprite = Gamepad.current != null ? controllerSprite : keyboardSprite;
        InputSystem.onDeviceChange += OnDeviceChange;
    }
    
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        spriteRenderer.sprite = device is Gamepad ? controllerSprite : keyboardSprite;
    }
    
    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}