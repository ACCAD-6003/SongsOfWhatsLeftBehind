using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ControllerBasedImage : MonoBehaviour
{
    [SerializeField] private Sprite controllerSprite;
    [SerializeField] private Sprite keyboardSprite;
    
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    
    private void Start()
    {
        image.sprite = Gamepad.current != null ? controllerSprite : keyboardSprite;
        InputSystem.onDeviceChange += OnDeviceChange;
    }
    
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        image.sprite = device is Gamepad ? controllerSprite : keyboardSprite;
    }
    
    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}