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
            
            while (MoreLinesToProcess())
            {
                var line = NextLine();
                if (line.StartsWith(PhraseLabel))
                {
                    var phrase = new PhraseData();
                    phrase.phraseName = line[PhraseLabel.Length..].Split(" ")[0];
                    phrase.startTime = ConvertTimeToOffset(line[PhraseLabel.Length..].Split(" ")[1]);
                    RemoveLine();
                    while (MoreLinesToProcess() && !NextLine().StartsWith(PhraseLabel))
                    {
                        var note = new NoteData();
                        for(int i = 0; i < NextLine().Split().Length - 1; i++)
                        {
                            if(Enum.TryParse(typeof(NoteType), NextLine().Split()[i], true, out var noteType)) 
                                note.notes.Add((NoteType)noteType);
                        }
                        note.offset = ConvertTimeToOffset(NextLine().Split().Last());
                        RemoveLine();
                        phrase.notes.Add(note);
                    }
                    song.phrases.Add(phrase);
                }
            }

            return song;
        }
        
        private static float ConvertTimeToOffset(string time)
        {
            var timeParts = time.Split(':');
            var minutes = float.Parse(timeParts[0]);
            var seconds = float.Parse(timeParts[1]);
            var milliseconds = float.Parse(timeParts[2]);
            return (minutes * 60 + seconds + milliseconds / 1000);
        }

        private static void AssertMarker(string text, string marker)
        {
            Debug.Assert(text.StartsWith(marker), $"ERROR: {text} did not start with {marker}");
        }
    }
}
