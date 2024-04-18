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
        public static Action<ConversationData> OnDialogueStarted;
        public static Action OnDialogueEnded;
        public static Action<DialogueData> OnTextSet;
        public static Action<string> OnTextUpdated;
        public static Action<List<string>> OnChoiceMenuOpen;
        public static Action OnChoiceMenuClose;
        public static Action<string> OnAudioCue;
        
        // Triggered when an event dialogue is reached, will exit the dialogue first then trigger the event
        public static Action<string> OnEventTriggered;

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
        [Button] private void ClearWorldState() => WorldState.ClearAllStates();

        protected override void Awake()
        {
            base.Awake();
            conversationGroup = Resources.LoadAll<SOConversationData>("Dialogue").ToList();
            conversationGroup.Sort((x, y) => x.Data.StateRequirements.Count > y.Data.StateRequirements.Count ? -1 : 1);
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += DestroyOnStart;
        }

        private void DestroyOnStart(Scene _, Scene __)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                dialogueProgress.Clear();
                WorldState.ClearAllStates();
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= DestroyOnStart;
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
        
        public void StartDialogueName(string dialogueId)
        {
            if (inDialogue) return;
            
            inDialogue = true;
            UIController.Instance.SwapToUI();
            
            AdvanceDialogue(dialogueId);
            StartDialogue(dialogueId);
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

            var conversationDataPointer = conversationGroup.Find(data => data.Data.ID.ToLower().Equals(dialogueId.ToLower()) && CheckStateRequirements(data.Data));
            if (conversationDataPointer == null)
            {
                Debug.Log("Could not find " + dialogueId + " in database with valid condition");
                ExitDialogue();
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
            OnDialogueStarted?.Invoke(data);
            yield return DisplayDialogue(data);
            UpdateWorldState(data);
            yield return ProceedToNextDialogue(data);
        }

        private IEnumerator ProceedToNextDialogue(ConversationData data)
        {
            yield return AwaitChoice(data);
            var nextDialogue = choiceSelected == -1 ? "end" : 
                data.LeadsTo.Where(x => CheckStateRequirements(x.nextID)).ToList()[choiceSelected].nextID;
            
            if (nextDialogue.ToLower().Equals("end"))
            {
                Debug.Log("Exiting dialogue");
                ExitDialogue();
            }
            else if (data.LeadsTo.Where(x => CheckStateRequirements(x.nextID)).ToList()[choiceSelected].isEvent)
            {
                ExitDialogue();
                OnEventTriggered?.Invoke(nextDialogue);
                Debug.Log("Firing event: " + nextDialogue);
            }
            else
                StartDialogue(nextDialogue);
        }

        private static void UpdateWorldState(ConversationData data)
        {
            foreach (var change in data.StateChanges)
            {
                WorldState.SetState(change.State, change.Modifier);
                Debug.Log("Updating " + change.State + " to " + WorldState.GetState(change.State));
            }
        }

        public bool CheckStateRequirements(string dialogueID)
        {
            var data = conversationGroup.Where(x => x.Data.ID == dialogueID).ToList();
            if (!data.Any()) return true;

            return data
                .Select(x => x.Data.StateRequirements)
                .Any(y => y.All(requirement => requirement.IsMet(WorldState.GetState(requirement.State))));
        }
        
        public bool CheckStateRequirements(ConversationData data)
        {
            return data.StateRequirements.All(requirement => requirement.IsMet(WorldState.GetState(requirement.State)));
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
                OnChoiceMenuOpen?.Invoke(choices.Select(x => x.prompt).ToList());
                yield return new WaitUntil(() => choiceSelected != -1);
                OnChoiceMenuClose?.Invoke();
            }
        }

        private IEnumerator DisplayDialogue(ConversationData data)
        {
            abortDialogue = false;
            UIController.OnOverrideSkip += OnAbort;
            if (!string.IsNullOrEmpty(data.AudioCue))
            {
                OnAudioCue?.Invoke(data.AudioCue);
            }

            var dialogueIndex = dialogueProgress.TryGetValue(data.ID, out var progress)
                ? Mathf.Min(progress, data.DialoguesSeries.Count - 1)
                : 0;
            var dialogues = data.DialoguesSeries[dialogueIndex].dialogues;

            foreach (var dialogue in dialogues.TakeWhile(_ => !abortDialogue))
            {
                yield return ProcessDialogue(dialogue);
            }

            UIController.OnOverrideSkip -= OnAbort;
        }

        private void OnContinueInput() => continueInputReceived = true;

        private IEnumerator ProcessDialogue(DialogueData dialogue)
        {
            var speakerName = dialogue.speakerName;
            
            OnTextSet?.Invoke(dialogue);
            OnTextUpdated?.Invoke("");
            yield return new WaitUntil(() => FadeToBlackSystem.FadeOutComplete);

            continueInputReceived = false;

            UIController.OnNextDialogue += SpeedUpText;
            yield return TypewriterDialogue(speakerName, dialogue);
            UIController.OnNextDialogue -= SpeedUpText;
            
            UIController.OnNextDialogue += OnContinueInput;
            yield return new WaitUntil(() => continueInputReceived);
            UIController.OnNextDialogue -= OnContinueInput;
        }

        private IEnumerator TypewriterDialogue(string name, DialogueData dialogue)
        {
            currentDialogueSpeed = dialogueSpeed;
            var loadedText = "";
            var atSpecialCharacter = false;
            var line = dialogue.Dialogue;
            
            for (var index = 0; index < line.Length; index++)
            {
                var letter = line[index];

                loadedText += letter;
                atSpecialCharacter = letter == '<' || atSpecialCharacter;
                if (atSpecialCharacter && letter != '>') continue;
                atSpecialCharacter = false;
                OnTextUpdated?.Invoke(loadedText);
                yield return new WaitForSeconds(1 / currentDialogueSpeed);
                
                if (!abortDialogue) continue;
                OnTextUpdated?.Invoke(name + line);
                break;
            }
            
            playersReady++;
        }

        private void SpeedUpText() => currentDialogueSpeed = Math.Abs(currentDialogueSpeed - dialogueFastSpeed) < Mathf.Epsilon 
            ? dialogueFastSpeed * 10 : dialogueFastSpeed;
    }
}
