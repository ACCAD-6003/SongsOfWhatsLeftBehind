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

            string filePath = $"Assets/Resources/Dialogue/{conversation.Data.ID}.asset";
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
        //void AddChoicesToChain() => conversation.DialoguesSeries[^1].choices.Add(NextLine());

        conversation.ID = NextLine();
        Debug.Log($"Converting {NextLine()}");
        RemoveLine();
        
        AssertMarker(NextLine(), CONVERSANT_MARKER);
        conversation.Conversant = NextLine()[CONVERSANT_MARKER.Length..];
        RemoveLine();

        while (!ReachedLeadsTo())
        {
            AssertMarker(NextLine(), DIALOGUE_MARKER);
            RemoveLine();
            
            CreateNewChain();

            while (!ReachedChoices())
            {
                AddDialogueToChain(conversation, NextLine());
                RemoveLine();
            }
            
            RemoveLine();
            
            while (!ReachedDialogue() && !ReachedLeadsTo())
            {
                //AddChoicesToChain();
                RemoveLine();
            }
        }
        
        AssertMarker(NextLine(), LEADS_TO_MARKER);
        if(!NextLine().Trim().EndsWith(LEADS_TO_MARKER))
            conversation.LeadsTo.Add(NextLine()[LEADS_TO_MARKER.Length..].Trim());
        RemoveLine();

        while (MoreLinesToProcess())
        {
            conversation.LeadsTo.Add(NextLine());
            RemoveLine();
        }

        return conversation;
    }

    private static void AddDialogueToChain(ConversationData conversation, string line)
    {
        var markersToCheck = new List<(string label, ConversantType type)>
        {
            (PLAYER_MARKER, ConversantType.PlayerOne), (PLAYER_TWO_MARKER, ConversantType.PlayerTwo), 
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
