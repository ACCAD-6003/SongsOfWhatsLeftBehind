using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace RhythmGame
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private PointsDisplay pointsDisplay;
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text streakText;
        [SerializeField] private int maxCombo;
        [SerializeField] private int hitsPerCombo;
        
        [Header("Temp")]
        [SerializeField] private int maxScore;

        private int score;
        private int Combo => Mathf.Min(currentHits / hitsPerCombo + 1, maxCombo);
        private int currentHits;

        private void OnEnable()
        {
            score = 0;
            currentHits = 0;
            UpdateDisplay();
        }

        public void OnScoreNote(NoteResult result)
        {
            switch (result)
            {
                case NoteResult.Great:
                    score += 50 * Combo;
                    currentHits++;
                    break;
                case NoteResult.Ok:
                    score += 25 * Combo;
                    currentHits++;
                    break;
                case NoteResult.Miss:
                    currentHits = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }

            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            pointsDisplay.UpdateScore(score, maxScore);
            comboText.text = "Combo: " + Combo + "x";
            streakText.text = currentHits + " Hits!";
            streakText.enabled = currentHits != 0;
        }
    }
}