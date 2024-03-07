using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Dialogue_System
{
    public static class DialogueHelperClass
    {
        public static readonly string ID_MARKER = "ID: ";
        public static readonly string CONVERSANT_MARKER = "Conversant: ";
        public static readonly string UNLOCKS_MARKER = "Unlocks: ";
        public static readonly string DIALOGUE_MARKER = "Dialogue:";
        public static readonly string PLAYER_MARKER = "Uri: ";
        public static readonly string PLAYER_TWO_MARKER = "Centa: ";
        public static readonly string VOICE_MARKER = "Voice: ";
        public static readonly string CHOICES_MARKER = "Choices:";
        public static readonly string LEADS_TO_MARKER = "Leads To:";
        public static readonly string SOUND_MARKER = "Music:";
        public static readonly string EMPTY_MARKER = "N/A";
        public static readonly string POTION_MADE_UNLOCK = "ITEMCRAFTED";
        public static readonly string SUCCESS = "SUCCESS";
        public static readonly string FAILURE = "FAIL";
        public static readonly string PLAYER_SPEAKING_TO_EACH_OTHER_LABEL = "Player";
        public static readonly List<string> PREFIXES = new List<string>() { "F", "B", "A" };

        public static List<string> POTION_GIVEN_MARKERS => PREFIXES.Select(p => p + SUCCESS).Concat(PREFIXES.Select(p => p + FAILURE)).Select(p => p.ToLower()).ToList();


        [System.Serializable]
        public class DialogueData
        {
            public ConversantType speaker;
            [SerializeField, TextArea()] public string Dialogue;
        }
    
        public enum ConversantType
        {
            PlayerOne,
            PlayerTwo,
            Conversant,
            Other,
        }

        [System.Serializable]
        public class ConversationData
        {
            public string ID;
            public string Conversant;
            public List<DialogueChain> DialoguesSeries = new();
            public List<string> LeadsTo = new();
        }

        [System.Serializable]
        public class DialogueChain
        {
            public List<DialogueData> dialogues = new();
        }
    }
}