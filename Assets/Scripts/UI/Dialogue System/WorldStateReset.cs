using UnityEngine;

namespace UI.Dialogue_System
{
    public class WorldStateReset : MonoBehaviour
    {
        public static void ResetWorldState()
        {
            WorldState.ClearAllStates();
        }
    }
}