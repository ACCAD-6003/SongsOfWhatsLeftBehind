using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RhythmGame
{
    public class RhythmGameManager : SerializedMonoBehaviour
    {
        private const int NOTE_POOL_SIZE = 10;
        
        [SerializeField] private SongData songData;
        [SerializeField] private float noteDelay;
        [SerializeField] private float correctThreshold;
        [SerializeField] private float timeToReachBottom;
        [SerializeField] private float targetZone;
        [SerializeField] private Dictionary<NoteType, GameObject> notePrefabs;
        [SerializeField] private Dictionary<NoteType, Vector2> notePositions;

        Dictionary<NoteType, List<NoteImage>> notePools = new();
        GameObject noteContainer;

        [HideInInspector] public Action OnSongStart;
        [HideInInspector] public Action OnSongEnd;

        private void Start()
        {
            CreateNotePools();
        }

        private void CheckForNoteInZone(NoteType type)
        {
            var notesInZone = notePools[type]
                .Where(x => x.gameObject.activeInHierarchy)
                .Where(x => x.transform.position.y <= targetZone + correctThreshold &&
                            x.transform.position.y >= targetZone - correctThreshold)
                .ToList();
            if (notesInZone.Count > 0)
            {
                foreach (var note in notesInZone)
                {
                    note.gameObject.SetActive(false);
                    Debug.Log("Score");
                }
            }
            else
            {
                Debug.Log("Lose Score");
            }
        }
        
        private void DisplayNote(NoteType noteType)
        {
            var note = GetNoteFromPool(noteType);
            note.transform.position = notePositions[noteType];
            note.Send(timeToReachBottom, targetZone);
        }
        
        private void CreateNotePools()
        {
            notePools.Clear();
            noteContainer = new GameObject("NoteContainer");
            
            foreach (var noteType in Enum.GetValues(typeof(NoteType)))
            {
                var notes = new List<NoteImage>();
                for (int i = 0; i < NOTE_POOL_SIZE; i++)
                {
                    var note = Instantiate(notePrefabs[(NoteType)noteType], noteContainer.transform, true);
                    note.SetActive(false);
                    notes.Add(note.GetComponent<NoteImage>());
                }
                notePools.Add((NoteType)noteType, notes);
            }
        }

        private NoteImage GetNoteFromPool(NoteType noteType)
        {
            foreach (var note in notePools[noteType].Where(note => !note.gameObject.activeInHierarchy))
            {
                note.gameObject.SetActive(true);
                return note;
            }
            
            var newNote = Instantiate(notePrefabs[noteType], noteContainer.transform, true).GetComponent<NoteImage>();
            newNote.gameObject.SetActive(true);
            notePools[noteType].Add(newNote);
            return newNote;
        }
        
        [Button]
        public void StartSong()
        {
            OnSongStart?.Invoke();
            
            if(IsValidSong(songData)) StartCoroutine(HandleSong(songData));
        }

        private bool IsValidSong(SongData song)
        {
            var isValidSong = song.phrases[0].notes[0].offset + song.phrases[0].startTime - timeToReachBottom >= 0;
            Debug.Assert(isValidSong, "Not enough time to reach bottom");
            return isValidSong;
        }

        private IEnumerator HandleSong(SongData songData)
        {
            Controller.UIController.OnNotePressed += CheckForNoteInZone;
            Controller.UIController.Instance.SwapToUI();
            var startTime = Time.realtimeSinceStartup;
            float TimeElapsed() => Time.realtimeSinceStartup - startTime;
            foreach (var phrase in songData.phrases)
            {
                yield return new WaitUntil(() => TimeElapsed() >= phrase.startTime - timeToReachBottom);
                foreach (var note in phrase.notes)
                {
                    yield return new WaitUntil(() => TimeElapsed() >= phrase.startTime - timeToReachBottom + note.offset);
                    foreach (var noteType in note.notes)
                    {
                        DisplayNote(noteType);
                    }
                }
            }

            yield return new WaitUntil(() => !notePools.Any(x => x.Value.Any(y => y.gameObject.activeInHierarchy)));
            Controller.UIController.OnNotePressed -= CheckForNoteInZone;
            EndSong();
        }
        
        

        private void EndSong()
        {
            OnSongEnd?.Invoke();
        }
    }
}