using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Controller;
using Sirenix.Utilities;

namespace RhythmGame
{
    public class RhythmGameManager : SerializedMonoBehaviour
    {
        [HideInInspector] public Action OnSongStart;
        [HideInInspector] public Action OnSongEnd;

        private const int NOTE_POOL_SIZE = 30;
        
        [SerializeField] private SongData songData;
        [SerializeField] private float correctThreshold;
        [SerializeField, ReadOnly] private float timeToReachBottom;
        [SerializeField] private float targetZone;
        [SerializeField] private GameObject notePrefab;
        [SerializeField] private float yOffset;
        [SerializeField] private RhythmGameMusicPlayer musicPlayer;
        [SerializeField] private Dictionary<NoteType, NoteTemplate> templates;
        
        List<NoteImage> notePool = new();

        private IEnumerable<NoteImage> ActiveNotes => notePool
            .Where(y => y.gameObject.activeInHierarchy)
            .ToList();
        
        GameObject noteContainer;

        private void Start()
        {
            CreateNotePools();
        }

        private void CheckForNoteInZone(NoteType type)
        {
            var notesInZone = ActiveNotes
                .Where(x => WithinThreshold(x.Position))
                .ToList();
            if (notesInZone.Count == 0 || notesInZone.Any(x => (x.NoteType & type) == 0))
            {
                LoseScore();
            }
            else 
            {
                foreach (var note in notesInZone)
                {
                    note.Hit(Score, LoseScore);
                }
            }
        }

        private void Score()
        {
            Debug.Log("Score");
        }

        private void LoseScore()
        {
            Debug.Log("Lose Score");
            musicPlayer.PlayErrorSound();
        }

        private bool WithinThreshold(float value)
        {
            return Math.Abs(value - Mathf.Clamp(value, targetZone - correctThreshold, targetZone + correctThreshold)) 
                   < Mathf.Epsilon;
        }
        
        private void DisplayNote(NoteType noteType, NoteStyle style, float length)
        {
            var note = GetNoteFromPool(noteType, style);
            note.transform.position = new Vector3(templates[noteType].position, yOffset);
            note.Send(timeToReachBottom, targetZone, WithinThreshold, length);
        }
        
        private void CreateNotePools()
        {
            notePool.Clear();
            noteContainer = new GameObject("NoteContainer");
            for (int i = 0; i < NOTE_POOL_SIZE; i++)
            {
                var note = Instantiate(notePrefab, noteContainer.transform, true);
                note.SetActive(false);
                notePool.Add(note.GetComponent<NoteImage>());
            }
        }

        private NoteImage GetNoteFromPool(NoteType noteType, NoteStyle style = NoteStyle.Single)
        {
            var note = notePool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
            
            if (note == null)
            {
                note = Instantiate(notePrefab, noteContainer.transform, true).GetComponent<NoteImage>();
                notePool.Add(note);
            }
            
            note.gameObject.SetActive(true);
            note.Setup(templates[noteType], style);
            return note;
        }
        
        [Button]
        public void StartSong(float startBeat)
        {
            OnSongStart?.Invoke();
            
            if(IsValidSong(songData)) StartCoroutine(HandleSong(songData, RhythmHelper.ConvertBPMToOffset(startBeat, songData)));
        }

        [Button]
        public void RestartSong(float startBeat)
        {
            EndSong();
            StartSong(startBeat);
        }

        private bool IsValidSong(SongData song)
        {
            var isValidSong = song.phrases[0].notes[0].offset + song.phrases[0].startTime - timeToReachBottom >= 0;
            Debug.Assert(isValidSong, "Not enough time to reach bottom");
            return isValidSong;
        }

        private IEnumerator HandleSong(SongData songData, float songStart = 0)
        {
            timeToReachBottom = songData.speed;
            musicPlayer.PlaySong(songData, songStart);
            RhythmGameController.OnNotePressedProcessed += CheckForNoteInZone;
            UIController.Instance.SwapToUI();
            var startTime = Time.realtimeSinceStartup;
            float TimeElapsed() => Time.realtimeSinceStartup - startTime + songStart;
            foreach (var phrase in songData.phrases)
            {
                yield return new WaitUntil(() => TimeElapsed() >= phrase.startTime - timeToReachBottom);
                foreach (var note in phrase.notes.Where(x => x.offset + phrase.startTime - timeToReachBottom >= songStart))
                {
                    yield return new WaitUntil(() => TimeElapsed() >= phrase.startTime - timeToReachBottom + note.offset);
                    foreach (var noteType in note.notes)
                    {
                        DisplayNote(noteType, note.style, note.noteLength);
                    }
                }
            }

            //yield return new WaitUntil(() => !notePool.Any(y => y.gameObject.activeInHierarchy));
            yield return new WaitUntil(() => songData.song.length < TimeElapsed());
            EndSong();
        }
        
        

        [Button]
        private void EndSong()
        {
            Debug.Log("Ending Song");
            RhythmGameController.OnNotePressedProcessed -= CheckForNoteInZone;
            StopAllCoroutines();
            notePool.ForEach(x => x.gameObject.SetActive(false));
            musicPlayer.StopSong();
            OnSongEnd?.Invoke();
            UIController.Instance.SwapToGameplay();
        }
    }
}