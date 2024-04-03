using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using static RhythmGame.RhythmHelper;

namespace RhythmGame.Editor
{
    public static class RhythmConverter
    {
        public static void ConvertToJson(string text)
        {
            Debug.Log("Converting to Song");
            var song = ConvertToSong(text);

            string filePath = $"Assets/Resources/Songs/{song.SongName}.asset";
            if (System.IO.File.Exists(filePath))
            {
                var file = AssetDatabase.LoadAssetAtPath(filePath, typeof(SongData)) as SongData;
                song.song = file.song;
                EditorUtility.CopySerialized(song, file);
                EditorUtility.SetDirty(file);
            }
            else
            {
                AssetDatabase.CreateAsset(song, filePath);
            }
        }
    
        private static SongData ConvertToSong(string text)
        {
            var song = ScriptableObject.CreateInstance<SongData>();
            var lines = text.Split('\n').Where(x => !x.IsNullOrWhitespace()).Select(x => x.Trim()).ToList();
            string NextLine() => lines[0];
            void RemoveLine() => lines.RemoveAt(0);
            bool MoreLinesToProcess() => lines.Count > 0;

            AssertMarker(NextLine(), SongLabel);
            song.name = NextLine()[SongLabel.Length..];
            RemoveLine();
            
            AssertMarker(NextLine(), BPMLabel);
            var bpm = float.Parse(NextLine()[BPMLabel.Length..]);
            song.bpm = bpm;
            RemoveLine();
            
            if (NextLine().StartsWith(SpeedLabel))
            {
                song.speed = float.Parse(NextLine()[SpeedLabel.Length..]);
                RemoveLine();
            }
            
            AssertMarker(NextLine(), DialogueLabel);
            song.dialogue = NextLine()[DialogueLabel.Length..];
            RemoveLine();

            float BpmCalculator(string x) => ConvertBPMToOffset(x, bpm);

            while (MoreLinesToProcess())
            {
                var line = NextLine();
                if (line.StartsWith(PhraseLabel))
                {
                    var phrase = new PhraseData();
                    phrase.phraseName = line[PhraseLabel.Length..].Split(" ")[0];
                    phrase.startTime = BpmCalculator(line[PhraseLabel.Length..].Split(" ")[1]);
                    RemoveLine();
                    while (MoreLinesToProcess() && !NextLine().StartsWith(PhraseLabel))
                    {
                        var note = CreateTimeNote(NextLine().Split(" "), BpmCalculator);
                        RemoveLine();

                        phrase.notes.Add(note);
                    }
                    song.phrases.Add(phrase);
                }
            }

            return song;
        }

        private static NoteData CreateTimeNote(string[] splitLine, Func<string, float> offsetCalculator)
        {
            var note = new NoteData();
            int i;
            for (i = 0;
                 i < splitLine.Length - 1
                 && !float.TryParse(splitLine[i], out _)
                 && Enum.TryParse(typeof(NoteType), splitLine[i], true, out var noteType);
                 i++)
            {
                note.notes.Add((NoteType)noteType);
            }

            if (i != splitLine.Length - 1)
            {
                Debug.Log($"Held Note: {splitLine[^2]} to {splitLine[^1]}");
                note.style = NoteStyle.Hold;
                note.offset = offsetCalculator(splitLine[^2]);
                note.noteLength = offsetCalculator(splitLine[^1]) - note.offset;
            }
            else
            {
                Debug.Log($"Single Note: {splitLine.Last()}");
                note.style = NoteStyle.Single;
                note.offset = offsetCalculator(splitLine.Last());
            }

            return note;
        }

        private static float ConvertTimeToOffset(string time)
        {
            var timeParts = time.Split(':');
            Debug.Log(time);
            var minutes = int.Parse(timeParts[0]);
            var seconds = int.Parse(timeParts[1]);
            var milliseconds = timeParts.Length > 2 ? int.Parse(timeParts[2]) : 0;
            return (minutes * 60 + seconds + milliseconds / 1000f);
        }
        
        private static float ConvertBPMToOffset(string beat, float bpm)
        {
            return float.TryParse(beat, out var x) ? x * (60 / bpm) : ConvertTimeToOffset(beat);
        }

        private static void AssertMarker(string text, string marker)
        {
            Debug.Assert(text.StartsWith(marker), $"ERROR: {text} did not start with {marker}");
        }
    }
}
