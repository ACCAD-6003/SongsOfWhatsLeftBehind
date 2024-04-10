using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Dialogue_System
{
    public static class DialogueHelperClass
    {
        public static readonly string ID_MARKER = "ID: ";
        public static readonly string CONDITIONAL_MARKER = "Conditions: ";
        public static readonly string CHANGES_MARKER = "Changes: ";
        public static readonly string CONVERSANT_MARKER = "Conversant: ";
        public static readonly string UNLOCKS_MARKER = "Unlocks: ";
        public static readonly string DIALOGUE_MARKER = "Dialogue:";
        public static readonly string PLAYER_MARKER = "Kristen: ";
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
        public static readonly string EVENT_MARKER = "*";
        public static readonly string VARIATION_MARKER = "Variation: ";
        public static readonly string MUSIC_MARKER = "Cue Audio: ";

        public static List<string> POTION_GIVEN_MARKERS => PREFIXES.Select(p => p + SUCCESS).Concat(PREFIXES.Select(p => p + FAILURE)).Select(p => p.ToLower()).ToList();


        [Serializable]
        public class DialogueData
        {
            public ConversantType speaker;
            [SerializeField, TextArea()] public string Dialogue;
        }
    
        public enum ConversantType
        {
            Player,
            Conversant,
            Other,
        }

        [Serializable]
        public class ConversationData
        {
            public string ID;
            public string Conversant;
            public List<DialogueChain> DialoguesSeries = new();
            public List<LeadsToPath> LeadsTo = new();
            public List<StateChange> StateChanges = new();
            public List<StateRequirement> StateRequirements = new();
            public string Variation;
            public string AudioCue;
            public bool HasChoice => LeadsTo[0].prompt != "";
        }
        
        [Serializable]
        public class StateChange
        {
            [SerializeField, HideInInspector] List<string> components;
            [LabelText("Change: "), SerializeField, ReadOnly] private string inspectorDescription;
            [HideInInspector] public string State;

            public Func<int, int> Modifier
            {
                get
                {
                    if (components == null) return null;
                    switch (components.Count)
                    {
                        case 1:
                            var hasPrefix = components[0].StartsWith("!");
                            return hasPrefix ? _ => 0 : _ => 1;
                        case 3:
                            return components[1] switch
                            {
                                "=" => _ => int.Parse(components[2]),
                                "+=" => x => x + int.Parse(components[2]),
                                "-=" => x => x - int.Parse(components[2]),
                                _ => throw new ArgumentException("Invalid operator")
                            };
                        default:
                            throw new ArgumentException("Invalid condition");
                    }
                }
            }

            public StateChange(string conditionLine)
            {
                components = conditionLine.Split(" ").ToList();
                
                switch (components.Count)
                {
                    case 1:
                        var hasPrefix = components[0].StartsWith("!");
                        State = hasPrefix ? components[0][1..] : components[0];
                        break;
                    case 3:
                        State = components[0];
                        break;
                    default:
                        throw new ArgumentException("Invalid condition");
                }
                
                inspectorDescription = conditionLine;
            }
        }
        
        [Serializable]
        public class StateRequirement
        {
            [SerializeField, HideInInspector] private List<string> components;
            
            [SerializeField, ReadOnly, LabelText("Requirement: ")] private string inspectorDescription;
            [HideInInspector] public string State;
            
            public Predicate<int> IsMet
            {
                get
                {
                    if (components == null) return null;
                    switch (components.Count)
                    {
                        case 1:
                            var hasPrefix = components[0].StartsWith("!");
                            State = hasPrefix ? components[0][1..] : components[0];
                            return hasPrefix ? x => x == 0 : x => x != 0;
                        case 3:
                            State = components[0];
                            return GetOperatorPredicate(components[1], int.Parse(components[2]));
                        default:
                            throw new ArgumentException($"Invalid condition: {inspectorDescription} has {components.Count}");
                    }
                }
            }
            
            public StateRequirement(string conditionLine)
            {
                components = conditionLine.Split(" ").ToList();

                switch (components.Count)
                {
                    case 1:
                        var hasPrefix = components[0].StartsWith("!");
                        State = hasPrefix ? components[0][1..] : components[0];
                        break;
                    case 3:
                        State = components[0];
                        break;
                    default:
                        throw new ArgumentException("Invalid condition");
                }
                
                inspectorDescription = conditionLine;
            }

            private static Predicate<int> GetOperatorPredicate(string op, int state)
            {
                return op switch
                {
                    "==" => x => x == state,
                    "!=" => x => x != state,
                    ">" => x => x > state,
                    "<" => x => x < state,
                    ">=" => x => x >= state,
                    "<=" => x => x <= state,
                    _ => throw new ArgumentException("Invalid operator")
                };
            }
        }
        
        [Serializable]
        public class LeadsToPath
        {
            [SerializeField, ReadOnly, LabelText("Path: ")] private string inspectorDescription;
            [HideInInspector] public string prompt;
            [HideInInspector] public string nextID;
            [HideInInspector] public bool isEvent;
            
            public LeadsToPath(string prompt, string nextID, bool isEvent)
            {
                this.prompt = prompt;
                this.nextID = nextID;
                this.isEvent = isEvent;
                inspectorDescription = isEvent 
                    ? "Prompt: " + prompt + " Triggers Event: " + nextID
                    : "Prompt: " + prompt + " Leads to: " + nextID;
            }
        }

        [System.Serializable]
        public class DialogueChain
        {
            public List<DialogueData> dialogues = new();
        }
    }
}