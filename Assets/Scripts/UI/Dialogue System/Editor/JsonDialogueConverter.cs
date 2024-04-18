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
        string NextLine() => MoreLinesToProcess() ? lines[0] : "";
        void RemoveLine() => lines.RemoveAt(0);
        bool MoreLinesToProcess() => lines.Count > 0;
        void CreateNewChain() => conversation.DialoguesSeries.Add(new DialogueChain());
        bool ReachedDialogue() => NextLine().StartsWith(DIALOGUE_MARKER);
        bool ReachedLeadsTo() => NextLine().StartsWith(LEADS_TO_MARKER);
        string Marker() => NextLine().Split(":")[0].Trim();
        string MarkerText() => NextLine().Split(":")[1].Trim();

        conversation.ID = NextLine();
        Debug.Log($"Converting {NextLine()}");
        RemoveLine();
        
        var markerActions = new Dictionary<string, Action>
        {
            {VARIATION_MARKER, () => conversation.Variation = MarkerText()},
            {CONDITIONAL_MARKER, () => conversation.StateRequirements = GetCondition(MarkerText())},
            {CHANGES_MARKER, () => conversation.StateChanges = GetWorldStateChanges(MarkerText())},
            {MUSIC_MARKER, () => conversation.AudioCue = MarkerText()},
        };
        
        while (!ReachedDialogue() && !ReachedLeadsTo() && MoreLinesToProcess())
        {
            Debug.Assert(markerActions.ContainsKey(Marker()), $"Unknown marker {Marker()} in {conversation.ID}");
            if (markerActions.TryGetValue(Marker(), out var action))
            {
                action();
            }
            RemoveLine();
        }

        if (Marker() == DIALOGUE_MARKER)
        {
            RemoveLine();
            CreateNewChain();
            while (!ReachedLeadsTo() && MoreLinesToProcess())
            {
                AddDialogueToChain(conversation, NextLine());
                RemoveLine();
            }
        }

        if (NextLine().StartsWith(LEADS_TO_MARKER))
        {
            if (!NextLine().Trim().EndsWith(LEADS_TO_MARKER))
                conversation.LeadsTo.Add(GetChoice(NextLine()[LEADS_TO_MARKER.Length..].Trim()));
            RemoveLine();

            while (MoreLinesToProcess())
            {
                conversation.LeadsTo.Add(GetChoice(NextLine().Trim()));
                RemoveLine();
            }
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
        var dialogueChain = conversation.DialoguesSeries.Last().dialogues;
        if (line.Contains(":"))
        {
            dialogueChain.Add(new DialogueData(line));
        }
        else
        {
            dialogueChain.Last().AppendDialogue(line);
        }
    }

    private static void AssertMarker(string text, string marker)
    {
        Debug.Assert(text.StartsWith(marker), $"ERROR: {text} did not start with {marker}");
    }
}
