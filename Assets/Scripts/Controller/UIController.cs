using System;
using System.Collections;
using RhythmGame;
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
        public static Action OnOpenQuestLog;

        public static Action<Vector2> OnNavigateMenu;
        public static Action OnSelect;
        public static Action OnGoBack;
        public static Action OnSkip;
        public static Action OnResume;
        public static Action OnNextDialogue;
        public static Action OnOverrideSkip;
        public static Action OnCloseQuestLog;
        public static Action<NoteType, bool> OnNotePressed;

        public static Action OnPlayerTwoInteract;

        public static Action OnCancel;

        public Action OnSwapToUI;
        public Action OnSwapToGameplay;

        [SerializeField, ReadOnly] private bool inGameplay = true;
        public bool InGameplay => inGameplay;

        [SerializeField] PlayerInput playerInput;

        public void SwapToUI() { playerInput.SwitchCurrentActionMap("UI"); OnSwapToUI?.Invoke(); inGameplay = false; }
        public void SwapToGameplay() { playerInput.SwitchCurrentActionMap("Gameplay"); OnSwapToGameplay?.Invoke(); inGameplay = true; }
        public string GetKey(string keyName)
        {
            try { return playerInput.actions.FindAction(keyName).GetBindingDisplayString(0); }
            catch { Debug.LogError("Can't find key " + keyName); return "?"; }
        }

        public IEnumerator AllowUserToSetKey(string keyName)
        {
            playerInput.enabled = false;
            var operation = playerInput.actions.FindAction(keyName).PerformInteractiveRebinding();
            Debug.Log("Press a key");
            operation.Start();
            yield return new WaitUntil(() => operation.completed);
            operation.Dispose();
            playerInput.enabled = true;
        }
        
        #region Gameplay Layout

        public void Interact(InputAction.CallbackContext context)
        {
            if (SceneTools.transistioning) return;
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
        
        public void OpenQuestLog(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnOpenQuestLog?.Invoke();
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
        
        public void CloseQuestLog(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnCloseQuestLog?.Invoke();
            }
        }

        #endregion

        #region KeyMaps

        public void FirstNote(InputAction.CallbackContext context)
        {
            BroadcastNote(context, NoteType.First);
        }
        
        public void SecondNote(InputAction.CallbackContext context)
        {
            BroadcastNote(context, NoteType.Second);
        }
        
        public void ThirdNote(InputAction.CallbackContext context)
        {
            BroadcastNote(context, NoteType.Third);
        }
        
        public void FourthNote(InputAction.CallbackContext context)
        {
            BroadcastNote(context, NoteType.Fourth);
        }

        private void BroadcastNote(InputAction.CallbackContext context, NoteType type)
        {
            if (context.started)
            {
                OnNotePressed?.Invoke(type, true);
            }
            else if (context.canceled)
            {
                OnNotePressed?.Invoke(type, false);
            }
        }

        #endregion
    }
}
