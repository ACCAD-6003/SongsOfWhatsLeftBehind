namespace RhythmGame
{
    public static class RhythmHelper
    {
        public static string SongLabel = "Song: ";
        public static string PhraseLabel = "Phrase: ";
        public static string BPMLabel = "BPM: ";
        
        public static float ConvertBPMToOffset(float beats, SongData song)
        {
            return (60f / song.bpm) * beats;
        }
    }
}