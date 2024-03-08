using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace RhythmGame
{
    public class NoteThreshold : MonoBehaviour
    {
        private readonly List<NoteImage> notes = new();

        private void OnEnable()
        {
            Controller.UIController.OnNotePressed += OnPress;
            notes.Clear();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out NoteImage note))
            {
                notes.Add(note);
                Debug.Log("Adding note");
            }
        }
        
        private void OnPress(NoteType pressedNote)
        {
            var notesToHide = notes.Where(note => pressedNote == note.NoteType).ToList();
            if (notesToHide.Any())
            {
                notesToHide.ForEach(x => x.gameObject.SetActive(false));
                Debug.Log("Scoring");
            }
            else
            {
                Debug.Log("Decreasing");
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out NoteImage note))
            {
                notes.Remove(note);
            }
        }
        
        private void OnDisable()
        {
            Controller.UIController.OnNotePressed -= OnPress;
        }
    }
}