using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    public class PointsDisplay : MonoBehaviour
    {
        [SerializeField] private List<Image> starImages;
        [SerializeField] private Image fillImage;

        private float value;

        private void OnEnable()
        {
            UpdateDisplay();
        }

        public void UpdateScore(float value, float maxValue)
        {
            this.value = value / maxValue;
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            fillImage.fillAmount = value;
            for (int i = 0; i < starImages.Count; i++)
            {
                starImages[i].color = value >= (i + 1) / (float) starImages.Count ? Color.green : Color.gray;
            }
        }
    }
}