using UnityEngine;

namespace RhythmGame
{
    [RequireComponent(typeof(RectTransform))]
    public class NoteConnector : MonoBehaviour
    {
        private RectTransform rectTransform;
        private NoteImage leftSide;
        private NoteImage rightSide;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Display(NoteImage leftSide, NoteImage rightSide)
        {
            this.leftSide = leftSide;
            this.rightSide = rightSide;
            UpdateTransform();
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (leftSide.gameObject.activeInHierarchy && rightSide.gameObject.activeInHierarchy)
            {
                UpdateTransform();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void UpdateTransform()
        {
            rectTransform.position = new Vector3(
                (rightSide.HorizontalPosition + leftSide.HorizontalPosition) / 2f,
                leftSide.VerticalPosition);
            rectTransform.sizeDelta = new Vector2(rightSide.HorizontalPosition - leftSide.HorizontalPosition,
                rectTransform.sizeDelta.y);
        }
    }
}