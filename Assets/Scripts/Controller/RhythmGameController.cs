using System;
using System.Collections;
using RhythmGame;
using UnityEngine;

namespace Controller
{
    public class RhythmGameController : MonoBehaviour
    {
        public static Action<NoteType> OnNoteProcessed;
        
        [SerializeField] private float delayForCombinedNotes = 0.05f;

        private bool isProcessingNote;
        private NoteType note;
        
        private void Start()
        {
            UIController.OnNotePressed += OnNotePressed;
        }
        
        private void OnNotePressed(NoteType noteType)
        {
            note |= noteType;
            if (!isProcessingNote) StartCoroutine(NoteQueue());
        }
        
        private IEnumerator NoteQueue()
        {
            isProcessingNote = true;
            yield return new WaitForSeconds(delayForCombinedNotes);
            Debug.Log("Pressing: " + note);
            OnNoteProcessed?.Invoke(note);
            note = 0;  
            isProcessingNote = false;
        }
        
        private void OnDestroy()
        {
            UIController.OnNotePressed -= OnNotePressed;
        }
    }
}