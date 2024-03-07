using UnityEngine;
using static UI.Dialogue_System.DialogueHelperClass;

namespace UI.Dialogue_System
{
    [CreateAssetMenu(fileName = "New Data", menuName = "Dialogue/Data")]
    public class SOConversationData : ScriptableObject
    {
        [SerializeField] ConversationData conversationData;

        public void SetConversation(ConversationData conversation)
        {
            conversationData = conversation;
        }

        public ConversationData Data => conversationData;
    }
}
