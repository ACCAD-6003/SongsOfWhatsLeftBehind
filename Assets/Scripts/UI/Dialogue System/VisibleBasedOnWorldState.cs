using UnityEngine;

namespace UI.Dialogue_System
{
    public class VisibleBasedOnWorldState : MonoBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private int value;
        [SerializeField] private bool invert;
        
        private void Start()
        {
            WorldState.OnWorldStateChanged += UpdateVisibility;
            UpdateVisibility();
        }

        private void OnDestroy()
        {
            WorldState.OnWorldStateChanged -= UpdateVisibility;
        }

        private void UpdateVisibility()
        {
            var shouldBeVisible = WorldState.GetState(key) == value;
            if (invert)
            {
                shouldBeVisible = !shouldBeVisible;
            }
            
            gameObject.SetActive(shouldBeVisible);
        }
    }
}