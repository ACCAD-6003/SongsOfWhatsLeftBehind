using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Controller;
using Sirenix.Utilities;
using UI.Dialogue_System;
using UnityEngine.UI;

namespace RhythmGame
{
    public class RhythmGameManager : SerializedMonoBehaviour
    {
        [HideInInspector] public Action<SongData> OnSongStart;
        [HideInInspector] public Action OnSongEnd;
        [HideInInspector] public Action OnHit;
        [HideInInspector] public Action OnMiss;
        [HideInInspector] public Action OnPulse;

        private const int NOTE_POOL_SIZE = 30;

        [SerializeField] private GameObject display;
        [SerializeField] private SongData songData;
        [SerializeField, ReadOnly] private float timeToReachBottom;
        [SerializeField] private RectTransform targetZone;
        [SerializeField] private GameObject notePrefab;
        [SerializeField] private GameObject noteConnectorPrefab;
        [SerializeField] private GameObject noteSpawnPositionMarker;
        [SerializeField] private RhythmGameMusicPlayer musicPlayer;
        [SerializeField] private Dictionary<NoteType, NoteTemplate> templates;
        [SerializeField] private PerformanceIndicator performanceIndicator;
        [SerializeField] private ScoreDisplay scoreDisplay;
        [SerializeField] private GameObject noteContainer;

        readonly List<NoteImage> notePool = new();
        readonly List<NoteConnector> noteConnectors = new();
        float CorrectThreshold => targetZone.rect.height / 1.5f;
        float ThresholdCenter => targetZone.position.y;
        float NoteSpawnPosition => noteSpawnPositionMarker.transform.position.y;
        private Coroutine pulser;

        private IEnumerable<NoteImage> ActiveNotes => notePool
            .Where(y => y.gameObject.activeInHierarchy)
            .ToList();
        
        private void Start()
        {
            CreateNotePools();
            ToggleDisplay(false);
        }

        private void CheckForNoteInZone(NoteType type)
        {
            var closestNote = ActiveNotes
                .Where(x => WithinThreshold(x.Position))
                .OrderBy(x => Mathf.Abs(x.Position - ThresholdCenter))
                .FirstOrDefault();
            if (closestNote == null)
            {
                LoseScore();
                return;
            }
            var notesInZone = ActiveNotes
                .Where(x => Mathf.Approximately(x.Position, closestNote.Position))
                .ToList();
            if (notesInZone.Any(x => (x.NoteType & type) == 0))
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
            performanceIndicator.Show(NoteResult.Great);
            scoreDisplay.OnScoreNote(NoteResult.Great);
            OnHit?.Invoke();
        }

        private void LoseScore()
        {
            musicPlayer.PlayErrorSound();
            performanceIndicator.Show(NoteResult.Miss);
            scoreDisplay.OnScoreNote(NoteResult.Miss);
            OnMiss?.Invoke();
        }

        private bool WithinThreshold(float value)
        {
            return Math.Abs(value - Mathf.Clamp(value, ThresholdCenter - CorrectThreshold, ThresholdCenter + CorrectThreshold)) 
                   < Mathf.Epsilon;
        }
        
        private NoteImage DisplayNote(NoteType noteType, NoteStyle style, float length)
        {
            var note = GetNoteFromPool(noteType, style);
            note.transform.position = new Vector3(display.transform.position.x + templates[noteType].position, NoteSpawnPosition);
            note.Send(timeToReachBottom, ThresholdCenter, WithinThreshold, length);

            return note;
        }

        private void CreateNotePools()
        {
            CreatePool(noteConnectors, noteConnectorPrefab, NOTE_POOL_SIZE / 3);
            CreatePool(notePool, notePrefab, NOTE_POOL_SIZE);
        }
        
        private void CreatePool<T>(ICollection<T> pool, GameObject prefab, int poolSize) where T : MonoBehaviour
        {
            pool.Clear();
            for (int i = 0; i < poolSize; i++)
            {
                var obj = Instantiate(prefab, noteContainer.transform, true);
                obj.SetActive(false);
                pool.Add(obj.GetComponent<T>());
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
            note.Setup(templates[noteType], LoseScore, style);
            return note;
        }
        
        private void SetupConnector(NoteImage left, NoteImage right)
        {
            var connector = noteConnectors.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
            if (connector == null)
            {
                connector = Instantiate(noteConnectorPrefab, noteContainer.transform, true).GetComponent<NoteConnector>();
                noteConnectors.Add(connector);
            }
            connector.Display(left, right);
        }
        
        [Button]
        public void StartSong(float startBeat)
        {
            OnSongStart?.Invoke(songData);

            if(IsValidSong(songData)) StartCoroutine(HandleSong(songData, RhythmHelper.ConvertBPMToOffset(startBeat, songData)));
        }

        public void PlaySong(SongData song)
        {
            songData = song;
            StartSong(0);
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

        private void ToggleDisplay(bool shouldDisplay)
        {
            display.SetActive(shouldDisplay);
        }

        private IEnumerator HandleSong(SongData songData, float songStart = 0)
        {
            UIController.OnOverrideSkip += EndSong;
            timeToReachBottom = songData.speed;
            ToggleDisplay(true);
            musicPlayer.PlaySong(songData, songStart);
            RhythmGameController.OnNotePressedProcessed += CheckForNoteInZone;
            UIController.Instance.SwapToUI();
            var startTime = Time.time;
            scoreDisplay.SetMaxScore(songData.maxScore);
            pulser = StartCoroutine(Pulser(songData.bpm, songData.song.length - songStart));
            float TimeElapsed() => Time.time - startTime + songStart;
            foreach (var phrase in songData.phrases)
            {
                yield return new WaitUntil(() => TimeElapsed() >= phrase.startTime - timeToReachBottom);
                foreach (var note in phrase.notes.Where(x => x.offset + phrase.startTime - timeToReachBottom >= songStart))
                {
                    yield return new WaitUntil(() => TimeElapsed() >= phrase.startTime - timeToReachBottom + note.offset);
                    var sentNotes = new List<NoteImage>();
                    foreach (var noteType in note.notes)
                    {
                        sentNotes.Add(DisplayNote(noteType, note.style, note.noteLength));
                    }
                    
                    for (int i = 0; i < sentNotes.Count - 1; i++)
                    {
                        SetupConnector(sentNotes[i], sentNotes[i + 1]);
                    }
                }
            }

            //yield return new WaitUntil(() => !notePool.Any(y => y.gameObject.activeInHierarchy));
            yield return new WaitUntil(() => songData.song.length < TimeElapsed());
            EndSong();
        }

        private IEnumerator Pulser(float bpm, float songLength)
        {
            float timePassed = 0;
            while (timePassed < songLength)
            {
                timePassed += Time.deltaTime;
                OnPulse?.Invoke();
                yield return new WaitForSeconds(60 / bpm);
            }
        }

        [Button]
        private void EndSong()
        {
            if (pulser != null)
            {
                StopCoroutine(pulser);
            }
            UIController.OnOverrideSkip -= EndSong;
            Debug.Log("Ending Song");
            RhythmGameController.OnNotePressedProcessed -= CheckForNoteInZone;
            StopAllCoroutines();
            notePool.ForEach(x => x.gameObject.SetActive(false));
            noteConnectors.ForEach(x => x.gameObject.SetActive(false));
            musicPlayer.StopSong();
            int finalScore = scoreDisplay.FinalScore;
            if (songData.mainSong) WorldState.SetState("Friendship", x => x + finalScore);
            OnSongEnd?.Invoke();
            ToggleDisplay(false);
            DialogueManager.Instance.StartDialogue(songData.dialogue);
        }
    }
}