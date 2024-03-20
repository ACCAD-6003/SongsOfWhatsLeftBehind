//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/Controller/Player Movement.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerMovement : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerMovement()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Movement"",
    ""maps"": [
        {
            ""name"": ""Player Acions"",
            ""id"": ""323a8a8f-c95e-45be-90de-bdf9654f0f58"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""a3cae127-38ed-4da6-9662-8f78c9d963f1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""e06936d8-f59e-4c76-b2a9-310335347d8f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9d769719-3e43-4a85-a6ad-125eb651ec96"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""4c43d23f-35a3-43ed-975e-98617bdfde21"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e1101bf7-990f-4d04-8504-7044d0eb59ca"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player Acions
        m_PlayerAcions = asset.FindActionMap("Player Acions", throwIfNotFound: true);
        m_PlayerAcions_Movement = m_PlayerAcions.FindAction("Movement", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player Acions
    private readonly InputActionMap m_PlayerAcions;
    private IPlayerAcionsActions m_PlayerAcionsActionsCallbackInterface;
    private readonly InputAction m_PlayerAcions_Movement;
    public struct PlayerAcionsActions
    {
        private @PlayerMovement m_Wrapper;
        public PlayerAcionsActions(@PlayerMovement wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PlayerAcions_Movement;
        public InputActionMap Get() { return m_Wrapper.m_PlayerAcions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerAcionsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerAcionsActions instance)
        {
            if (m_Wrapper.m_PlayerAcionsActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerAcionsActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerAcionsActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerAcionsActionsCallbackInterface.OnMovement;
            }
            m_Wrapper.m_PlayerAcionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
            }
        }
    }
    public PlayerAcionsActions @PlayerAcions => new PlayerAcionsActions(this);
    public interface IPlayerAcionsActions
    {
        void OnMovement(InputAction.CallbackContext context);
    }
}
