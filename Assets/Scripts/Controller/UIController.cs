using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class UIController : SingletonMonoBehavior<UIController>
    {
        public static Action OnClick;
        public static Action OnInteract;
        public static Action OnPause;
        public static Action OnChitChat;
        public static Action OnSkipScene;

        public static Action<Vector2> OnNavigateMenu;
        public static Action OnSelect;
        public static Action OnGoBack;
        public static Action OnSkip;
        public static Action OnResume;
        public static Action OnNextDialogue;
        public static Action OnOverrideSkip;
    
        public static Action OnPlayerTwoInteract;

        public static Action OnCancel;

        public Action OnSwapToUI;
        public Action OnSwapToGameplay;

        [SerializeField, ReadOnly] private bool inGameplay = true;
        public bool InGameplay => inGameplay;

        [SerializeField] PlayerInput playerInput;

        public void SwapToUI() { playerInput.SwitchCurrentActionMap("UI"); OnSwapToUI?.Invoke(); inGameplay = false; }
        public void SwapToGameplay() { playerInput.SwitchCurrentActionMap("Gameplay"); OnSwapToGameplay?.Invoke(); inGameplay = true; }
        
        #region Gameplay Layout

        public void Interact(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnInteract?.Invoke();
            }
        }

        public void Pause(InputAction.CallbackContext context) 
        {
            if (context.started)
            {
                OnPause?.Invoke();
            }
        }
        
        public void Click(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnClick?.Invoke();
            }
        }
        
        public void SkipScene(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSkipScene?.Invoke();
            }
        }

        #endregion

        #region UI Layout

        public void Resume(InputAction.CallbackContext context)
        {
            if (context.started) 
            {
                OnResume?.Invoke();
            }
        }

        public void NavigateMenu(InputAction.CallbackContext context)
        {
            if(context.started)
                OnNavigateMenu?.Invoke(context.ReadValue<Vector2>());
        }

        public void GoBack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnGoBack?.Invoke();
            }
        }
        
        public void ChitChat(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnChitChat?.Invoke();
            }
        }

        public void Select(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSelect?.Invoke();
            }
        }

        public void Skip(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSkip?.Invoke();
            }
        }

        public void NextDialogue(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnNextDialogue?.Invoke();
            }
        }

        public void OverrideSkip(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnOverrideSkip?.Invoke();
            }
        }

        #endregion

        #region UIPuzzle Layout
        public void Cancel(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnCancel?.Invoke();
            }
        }
        #endregion

        #region PlayerTwo Layout
    
        public void PlayerTwoInteract(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnPlayerTwoInteract?.Invoke();
            }
        }

        #endregion
    }
}
