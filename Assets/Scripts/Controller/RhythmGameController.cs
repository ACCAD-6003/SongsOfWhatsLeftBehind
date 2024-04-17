using System;
using System.Collections;
using RhythmGame;
using UnityEngine;

namespace Controller
{
    
    
    
    public class RhythmGameController : MonoBehaviour
    {
        public static Action<NoteType> OnNotePressedProcessed;
        public static Action<NoteType> OnNoteReleasedProcessed;
        
        [SerializeField] private float delayForCombinedNotes = 0.05f;

        private bool isProcessingPressed;
        private bool isProcessingRelease;
        private NoteType pressedNote;
        private NoteType releasedNote;
        
        private void Start()
        {
            UIController.OnNotePressed += OnNoteChanged;
        }
        
        private void OnNoteChanged(NoteType noteType, bool isPressed)
        {
            if (isPressed) OnNotePressed(noteType);
            else OnNoteReleased(noteType);
        }

        private void OnNoteReleased(NoteType noteType)
        {
            releasedNote |= noteType;
            if (!isProcessingRelease) StartCoroutine(NoteProcessingQueue(false));
        }
        
        private void OnNotePressed(NoteType noteType)
        {
            pressedNote |= noteType;
            if (!isProcessingPressed) StartCoroutine(NoteProcessingQueue(true));
        }
        
        private IEnumerator NoteProcessingQueue(bool isPressed)
        {
            if (isPressed) isProcessingPressed = true;
            else isProcessingRelease = true;
            yield return new WaitForSeconds(delayForCombinedNotes);
            if (isPressed)
            {
                OnNotePressedProcessed?.Invoke(pressedNote);
                pressedNote = 0;
                isProcessingPressed = false;
            }
            else
            {
                OnNoteReleasedProcessed?.Invoke(releasedNote);
                releasedNote = 0;
                isProcessingRelease = false;
            }
        }
        
        private void OnDestroy()
        {
            UIController.OnNotePressed -= OnNoteChanged;
        }
    }
}