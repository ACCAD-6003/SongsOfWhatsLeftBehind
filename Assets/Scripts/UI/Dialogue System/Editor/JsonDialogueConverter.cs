using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities;
using UI.Dialogue_System;
using UnityEditor;
using static UI.Dialogue_System.DialogueHelperClass;

public static class JsonDialogueConverter
{
    public static string ConvertToJson(ConversationData conversation)
    {
        return JsonUtility.ToJson(conversation, true);
    }

    public static void ConvertToJson(string text)
    {
        foreach (string dialogueScene in text.Split(ID_MARKER, StringSplitOptions.RemoveEmptyEntries)) {
            Debug.Log(dialogueScene);
            SOConversationData conversation = ScriptableObject.CreateInstance<SOConversationData>();
            conversation.SetConversation(ConvertFromJson(ConvertToJson(ConvertToConversation(dialogueScene))));

            string filePath = $"Assets/Resources/Dialogue/{conversation.name}.asset";
            if (System.IO.File.Exists(filePath))
            {
                var file = AssetDatabase.LoadAssetAtPath(filePath, typeof(SOConversationData)) as SOConversationData;
                file.SetConversation(conversation.Data);
                EditorUtility.SetDirty(file);
            }
            else
            {
                AssetDatabase.CreateAsset(conversation, filePath);
            }
        }
    }

    public static ConversationData ConvertFromJson(string jsonFile)
    {
        return JsonUtility.FromJson<ConversationData>(jsonFile);
    }

    public static ConversationData ConvertFromJson(TextAsset jsonFile)
    {
        return ConvertFromJson(jsonFile.text);
    }

    private static ConversationData ConvertToConversation(string text)
    {
        var conversation = new ConversationData();
        var lines = text.Split('\n').Where(x => !x.IsNullOrWhitespace()).Select(x => x.Trim()).ToList();
        string NextLine() => lines[0];
        void RemoveLine() => lines.RemoveAt(0);
        bool MoreLinesToProcess() => lines.Count > 0;
        void CreateNewChain() => conversation.DialoguesSeries.Add(new DialogueChain());
        bool ReachedChoices() => NextLine().StartsWith(CHOICES_MARKER);
        bool ReachedDialogue() => NextLine().StartsWith(DIALOGUE_MARKER);
        bool ReachedLeadsTo() => NextLine().StartsWith(LEADS_TO_MARKER);

        conversation.ID = NextLine();
        Debug.Log($"Converting {NextLine()}");
        RemoveLine();

        if (NextLine().StartsWith(VARIATION_MARKER))
        {
            conversation.Variation = NextLine()[VARIATION_MARKER.Length..];
            RemoveLine();
        }

        AssertMarker(NextLine(), CONDITIONAL_MARKER);
        conversation.StateRequirements = GetCondition(NextLine()[CONDITIONAL_MARKER.Length..]);
        RemoveLine();
        
        AssertMarker(NextLine(), CHANGES_MARKER);
        conversation.StateChanges = GetWorldStateChanges(NextLine()[CHANGES_MARKER.Length..]);
        RemoveLine();
        
        AssertMarker(NextLine(), CONVERSANT_MARKER);
        conversation.Conversant = NextLine()[CONVERSANT_MARKER.Length..];
        RemoveLine();

        if (NextLine().StartsWith(MUSIC_MARKER))
        {
            conversation.AudioCue = NextLine()[MUSIC_MARKER.Length..];
            RemoveLine();
        }

        AssertMarker(NextLine(), DIALOGUE_MARKER);
        RemoveLine();
        CreateNewChain();
        while (!ReachedLeadsTo())
        {
            AddDialogueToChain(conversation, NextLine());
            RemoveLine();
        }
        
        AssertMarker(NextLine(), LEADS_TO_MARKER);
        if(!NextLine().Trim().EndsWith(LEADS_TO_MARKER))
            conversation.LeadsTo.Add(GetChoice(NextLine()[LEADS_TO_MARKER.Length..].Trim()));
        RemoveLine();

        while (MoreLinesToProcess())
        {
            conversation.LeadsTo.Add(GetChoice(NextLine().Trim()));
            RemoveLine();
        }

        return conversation;
    }
    
    /*
     * Can be in the following forms:
     * None
     * {stateName}
     * !{stateName}
     * {value} {operator} {stateName} {operator} {value}
     *
     * Also supports multiple conditions separated by "and" or "or"
     *
     * Value is an integer
     * StateName is a string that is not an integer
     * Where operator is one of: ==, !=, >, <, >=, <=
     */
    private static List<StateRequirement> GetCondition(string line)
    {
        var conditionals = new List<StateRequirement>();
        if (line == "None") return conditionals;
        
        var conditions = line.Split("and").Select(x => x.Trim()).ToList();
        conditionals.AddRange(conditions.Select(components => new StateRequirement(components)));

        return conditionals;
    }

    /*
     * Can be in the following forms:
     * None
     * {stateName}
     * !{stateName}
     * {stateName} = {value}
     * {stateName} += {value}
     * {stateName} -= {value}
     */
    private static List<StateChange> GetWorldStateChanges(string line)
    {
        var changes = new List<StateChange>();
        if (line == "None") return changes;
        
        var conditions = line.Split("and").Select(x => x.Trim()).ToList();
        changes.AddRange(conditions.Select(components => new StateChange(components)));

        return changes;
    }
    
    private static LeadsToPath GetChoice(string line)
    {
        var hasPrompt = line.Contains("=>");
        var prompt = hasPrompt ? line.Split("=>")[0].Trim() : "";
        var nextID = hasPrompt ? line.Split("=>")[1].Trim() : line.Trim();
        var isEvent = nextID.StartsWith(EVENT_MARKER);
        nextID = isEvent ? nextID[EVENT_MARKER.Length..] : nextID;
        return new LeadsToPath(prompt, nextID, isEvent);        
    }

    private static void AddDialogueToChain(ConversationData conversation, string line)
    {
        var markersToCheck = new List<(string label, ConversantType type)>
        {
            (PLAYER_MARKER, ConversantType.Player), 
            (VOICE_MARKER, ConversantType.Other), ($"{conversation.Conversant}: ", ConversantType.Conversant)
        };

        var markerFound = markersToCheck.FirstOrDefault(x => line.StartsWith(x.label));

        if (markerFound == default)
        {
            conversation.DialoguesSeries[^1].dialogues[^1].Dialogue += " " + line;
        }
        else
        {
            var dialogueLine = line[markerFound.label.Length..].Trim();
            var dialogueData = new DialogueData { Dialogue = dialogueLine, speaker = markerFound.type };

            conversation.DialoguesSeries[^1].dialogues.Add(dialogueData);
        }
    }

    private static void AssertMarker(string text, string marker)
    {
        Debug.Assert(text.StartsWith(marker), $"ERROR: {text} did not start with {marker}");
    }
}
