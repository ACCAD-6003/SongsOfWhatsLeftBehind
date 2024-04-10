using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace RhythmGame
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text streakText;
        [SerializeField] private int maxCombo;
        [SerializeField] private int hitsPerCombo;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float pulseDuration = .2f;
        [SerializeField] private float scaleMax = 1.2f;

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
            scoreText.text = "Score: " + score;
            comboText.text = "Combo: " + Combo + "x";
            streakText.text = currentHits + " Hits!";
            streakText.enabled = currentHits != 0;

            StopCoroutine(nameof(Pulse));
            StartCoroutine(nameof(Pulse));
        }

        private IEnumerator Pulse()
        {
            var timeToLive = 0f;
            var timeToPulse = 0f;
            transform.localScale = Vector3.one;
            
            while (timeToLive < duration)
            {
                timeToLive += Time.deltaTime;
                timeToPulse += Time.deltaTime;
                var scale = timeToPulse < pulseDuration 
                    ? Mathf.Lerp(1, scaleMax, timeToPulse / pulseDuration) 
                    : Mathf.Lerp(scaleMax, 1, (timeToLive - pulseDuration) / (duration - pulseDuration));
                
                transform.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }
        }
    }
}