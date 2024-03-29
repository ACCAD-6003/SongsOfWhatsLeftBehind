﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UI.Dialogue_System.DialogueHelperClass;

namespace UI.Dialogue_System
{
    public class DialogueManager : SingletonMonoBehavior<DialogueManager>
    {
        public const ConversantType PlayerOne = ConversantType.PlayerOne;
        public const ConversantType PlayerTwo = ConversantType.PlayerTwo;

        public static Action<ConversationData, ConversantType> OnDialogueStarted;
        public static Action OnDialogueEnded;
        public static Action<string, ConversantType, ConversantType> OnTextUpdated;
        public static Action<string, ConversantType, ConversantType> OnTextSet;
        public static Action<List<string>> OnChoiceMenuOpen;
        public static Action OnChoiceMenuClose;

        [SerializeField, Tooltip("Chars/Second")] float dialogueSpeed;
        [SerializeField, Tooltip("Chars/Second")] float dialogueFastSpeed;
        [SerializeField, ReadOnly] List<SOConversationData> conversationGroup;

        private readonly Dictionary<string, int> dialogueProgress = new();
    
        private float currentDialogueSpeed;
        private bool inDialogue;
        private bool continueInputReceived;
        private bool abortDialogue;
        private int choiceSelected;
        public bool InDialogue => inDialogue;
        public bool InInternalDialogue { get; private set; }

        public bool ValidateID(string id) => conversationGroup.Find(data => data.Data.ID.ToLower().Equals(id.ToLower()));
        private int playersReady;
    
        [Button] private void DisplayWorldState() => Debug.Log(WorldState.GetCurrentWorldState());
        [Button] private void SetWorldState(string key, int value) => WorldState.SetState(key, _ => value);

        protected override void Awake()
        {
            base.Awake();
            conversationGroup = Resources.LoadAll<SOConversationData>("Dialogue").ToList();
            conversationGroup.Sort((x, y) => x.Data.StateRequirements.Count > y.Data.StateRequirements.Count ? -1 : 1);
        }

        private void Start()
        {
            StartCoroutine(WatchForCrash());
            SceneManager.activeSceneChanged += DestroyOnStart;
        }

        private void DestroyOnStart(Scene _, Scene __)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                dialogueProgress.Clear();
                
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= DestroyOnStart;
        }

        private IEnumerator WatchForCrash()
        {
            while (true)
            {
                yield return new WaitUntil(() => inDialogue);
                while (!UIController.Instance.InGameplay)
                {
                    if (!inDialogue)
                    {
                        Debug.LogError("Dialogue system has crashed");
                        UIController.Instance.SwapToGameplay();
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        [Button(ButtonStyle.Box)]
        public void StartDialogue(SOConversationData conversation)
        {
            if (inDialogue) return;
            
            inDialogue = true;
            UIController.Instance.SwapToUI();
            
            AdvanceDialogue(conversation.Data.ID);
            StartDialogue(conversation.Data.ID);
        }
    
        private void AdvanceDialogue(string data)
        { 
            if (dialogueProgress.ContainsKey(data))
            {
                dialogueProgress[data]++;
            }
            else
            {
                dialogueProgress.Add(data, 0);
            }
        }

        public void StartDialogue(string dialogueId)
        {
            if (dialogueId == null || dialogueId.ToLowerInvariant().Equals("exit"))
            {
                ExitDialogue();
                return;
            }

            var conversationDataPointer = conversationGroup.Find(data => data.Data.ID.ToLower().Equals(dialogueId.ToLower()) && CheckStateRequirements(dialogueId));
            if (conversationDataPointer == null)
            {
                Debug.LogError("Could not find " + dialogueId + " in database");
                return;
            }

            Debug.Log("Starting conversation: " + conversationDataPointer.Data.ID);
            StartCoroutine(HandleConversation(conversationDataPointer.Data));
        }

        private void ExitDialogue()
        {
            inDialogue = false;
            Debug.Log("Player one leaving");
            OnDialogueEnded?.Invoke();
            UIController.Instance.SwapToGameplay();
        }

        private void OnAbort() 
        {
            abortDialogue = true;
            OnContinueInput();
        }

        private IEnumerator HandleConversation(ConversationData data)
        {
            OnDialogueStarted?.Invoke(data, PlayerOne);
            yield return DisplayDialogue(data);
            UpdateWorldState(data);
            yield return ProceedToNextDialogue(data);
        }

        private IEnumerator ProceedToNextDialogue(ConversationData data)
        {
            yield return AwaitChoice(data);
            var nextDialogue = choiceSelected == -1 ? "end" : data.LeadsTo[choiceSelected].nextID;

            if (nextDialogue.ToLower().StartsWith("end"))
                ExitDialogue();
            else
                StartDialogue(nextDialogue);
        }

        private static void UpdateWorldState(ConversationData data)
        {
            foreach (var change in data.StateChanges)
            {
                WorldState.SetState(change.State, change.Modifier);
            }
        }

        private bool CheckStateRequirements(string dialogueID)
        {
            var data = conversationGroup.Where(x => x.Data.ID == dialogueID).ToList();
            if (!data.Any()) return true;

            return data
                .Select(x => x.Data.StateRequirements)
                .Any(y => y.All(requirement => requirement.IsMet(WorldState.GetState(requirement.State))));
        }
        
        public void SelectChoice(int choice) => choiceSelected = choice;

        private IEnumerator AwaitChoice(ConversationData data)
        {
            choiceSelected = -1;
            if (!data.HasChoice)
            {
                choiceSelected = data.LeadsTo.FindIndex(x => CheckStateRequirements(x.nextID));
            }
            else
            {
                var choices = data.LeadsTo.Where(x => CheckStateRequirements(x.nextID)).ToList();
                OnChoiceMenuOpen?.Invoke(choices.Select(x => x.nextID).ToList());
                yield return new WaitUntil(() => choiceSelected != -1);
                OnChoiceMenuClose?.Invoke();
            }
        }

        private IEnumerator DisplayDialogue(ConversationData data)
        {
            abortDialogue = false;
            UIController.OnOverrideSkip += OnAbort;

            var dialogueIndex = dialogueProgress.TryGetValue(data.ID, out var progress)
                ? Mathf.Min(progress, data.DialoguesSeries.Count - 1)
                : 0;
            var dialogues = data.DialoguesSeries[dialogueIndex].dialogues;

            foreach (var dialogue in dialogues.TakeWhile(_ => !abortDialogue))
            {
                yield return ProcessDialogue(dialogue, data.Conversant);
            }

            UIController.OnOverrideSkip -= OnAbort;
        }

        private void OnContinueInput() => continueInputReceived = true;

        private IEnumerator ProcessDialogue(DialogueData dialogue, string conversant)
        {
            var speakerName = SpeakerName(dialogue, conversant);
            
            OnTextSet?.Invoke(speakerName + dialogue.Dialogue, ConversantType.PlayerOne, dialogue.speaker);
            OnTextUpdated?.Invoke(speakerName, ConversantType.PlayerOne, dialogue.speaker);
            yield return new WaitUntil(() => FadeToBlackSystem.FadeOutComplete);

            continueInputReceived = false;


            UIController.OnNextDialogue += SpeedUpText;
            yield return TypewriterDialogue(speakerName, PlayerOne, dialogue);
            UIController.OnNextDialogue -= SpeedUpText;

            UIController.OnNextDialogue += OnContinueInput;
            yield return new WaitUntil(() => continueInputReceived);
            UIController.OnNextDialogue -= OnContinueInput;
        }

        private static string SpeakerName(DialogueData dialogue, string conversant)
        {
            var speakerName = "";
            if (dialogue.speaker == ConversantType.Other) return speakerName;
            
            speakerName = dialogue.speaker switch
            {
                ConversantType.PlayerOne => PLAYER_MARKER,
                ConversantType.Conversant => conversant + ": ",
                _ => speakerName
            };
            speakerName = $"<u>{speakerName}</u>" + "\n";

            return speakerName;
        }

        private IEnumerator TypewriterDialogue(string name, ConversantType player, DialogueData dialogue)
        {
            currentDialogueSpeed = dialogueSpeed;
            var loadedText = name;
            var atSpecialCharacter = false;
            var line = dialogue.Dialogue;
            
            for (var index = 0; index < line.Length; index++)
            {
                var letter = line[index];

                loadedText += letter;
                atSpecialCharacter = letter == '<' || atSpecialCharacter;
                if (atSpecialCharacter && letter != '>') continue;
                atSpecialCharacter = false;
                OnTextUpdated?.Invoke(loadedText, player, dialogue.speaker);
                yield return new WaitForSeconds(1 / currentDialogueSpeed);
                
                if (!abortDialogue) continue;
                OnTextUpdated?.Invoke(name + line, player, dialogue.speaker);
                break;
            }
            
            playersReady++;
        }

        private void SpeedUpText() => currentDialogueSpeed = Math.Abs(currentDialogueSpeed - dialogueFastSpeed) < Mathf.Epsilon 
            ? dialogueFastSpeed * 10 : dialogueFastSpeed;
    }
}
