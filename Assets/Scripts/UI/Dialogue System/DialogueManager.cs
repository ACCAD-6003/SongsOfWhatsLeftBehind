using System;
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

        [SerializeField, Tooltip("Chars/Second")] float dialogueSpeed;
        [SerializeField, Tooltip("Chars/Second")] float dialogueFastSpeed;
        [SerializeField, ReadOnly] List<SOConversationData> conversationGroup;

        private readonly Dictionary<string, int> dialogueProgress = new();
    
        private float currentDialogueSpeed;
        private bool inDialogue;
        private bool continueInputReceived;
        private bool abortDialogue;
        public bool InDialogue => inDialogue;
        public bool InInternalDialogue { get; private set; }

        public bool ValidateID(string id) => conversationGroup.Find(data => data.Data.ID.ToLower().Equals(id.ToLower()));
        private int playersReady;
    

        protected override void Awake()
        {
            base.Awake();
            conversationGroup = Resources.LoadAll<SOConversationData>("Dialogue").ToList();
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

            var conversationDataPointer = conversationGroup.Find(data => data.Data.ID.ToLower().Equals(dialogueId.ToLower()));
            if (conversationDataPointer == null)
            {
                Debug.LogError("Could not find " + dialogueId + " in database");
                return;
            }

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
            OnDialogueStarted?.Invoke(data, PlayerTwo);

            abortDialogue = false;
            UIController.OnOverrideSkip += OnAbort;
        
            var dialogueIndex = dialogueProgress.TryGetValue(data.ID, out var progress)
                ? Mathf.Min(progress, data.DialoguesSeries.Count - 1)
                : 0;
            var dialogues = data.DialoguesSeries[dialogueIndex].dialogues;


            for (var index = 0; index < dialogues.Count; index++)
            {
                if (abortDialogue) break;
                var dialogue = dialogues[index];
                var speakingToEachOther = data.Conversant == PLAYER_SPEAKING_TO_EACH_OTHER_LABEL;
                switch (speakingToEachOther)
                {
                    case false when dialogue.speaker is PlayerOne:
                        index++;
                        InInternalDialogue = true;
                        yield return ProcessDialogue(dialogue, dialogues[index], data.Conversant);
                        continue;
                    case false when dialogue.speaker is PlayerTwo:
                        index++;
                        InInternalDialogue = true;
                        yield return ProcessDialogue(dialogues[index], dialogue, data.Conversant);
                        continue;
                    default:
                        InInternalDialogue = false;
                        yield return ProcessDialogue(dialogue, dialogue, data.Conversant);
                        break;
                }
            }

            UIController.OnOverrideSkip -= OnAbort;

            var nextDialogueIndex = dialogueProgress.TryGetValue(data.ID, out var p)
                ? Mathf.Min(p, data.LeadsTo.Count - 1)
                : 0;
            var nextDialogue = data.LeadsTo[nextDialogueIndex];
            
            if(nextDialogue.ToLower().StartsWith("end"))
                ExitDialogue();
            else
                StartDialogue(nextDialogue);
        }

        private void OnContinueInput() => continueInputReceived = true;

        private IEnumerator ProcessDialogue(DialogueData dialogue, DialogueData dialogueTwo, string conversant)
        {
            var speakerName = SpeakerName(dialogue, conversant);
            var speakerNameTwo = SpeakerName(dialogueTwo, conversant);
            
            OnTextSet?.Invoke(speakerName + dialogue.Dialogue, ConversantType.PlayerOne, dialogue.speaker);
            OnTextUpdated?.Invoke(speakerName, ConversantType.PlayerOne, dialogue.speaker);
            OnTextSet?.Invoke(speakerNameTwo + dialogueTwo.Dialogue, ConversantType.PlayerTwo, dialogueTwo.speaker);
            OnTextUpdated?.Invoke(speakerNameTwo, ConversantType.PlayerTwo, dialogueTwo.speaker);
            yield return new WaitUntil(() => FadeToBlackSystem.FadeOutComplete);

            continueInputReceived = false;


            UIController.OnNextDialogue += SpeedUpText;
            playersReady = 0; 
            StartCoroutine(TypewriterDialogue(speakerName, PlayerOne, dialogue));
            StartCoroutine(TypewriterDialogue(speakerNameTwo, PlayerTwo, dialogueTwo));
            yield return new WaitUntil(() => playersReady == 2);
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
                ConversantType.PlayerTwo => PLAYER_TWO_MARKER,
                ConversantType.Conversant => conversant + ": ",
                _ => speakerName
            };
            speakerName = Underline(speakerName) + "\n";

            return speakerName;

            string Underline(string text) => "<u>" + text + "</u>";
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
