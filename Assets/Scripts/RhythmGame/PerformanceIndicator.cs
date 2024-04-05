using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    public class PerformanceIndicator : MonoBehaviour
    {
        [SerializeField] private Sprite greatSprite;
        [SerializeField] private Sprite okSprite;
        [SerializeField] private Sprite missSprite;
        [SerializeField] private float duration;
        [SerializeField] private float pulseDuration;
        [SerializeField] private float scaleMax;
        
        private Image spriteRenderer;
        private float timeToLive;
        private float timeToPulse;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<Image>();
            gameObject.SetActive(false);
        }
        
        public void Show(NoteResult result)
        {
            gameObject.SetActive(true);
            
            spriteRenderer.sprite = result switch
            {
                NoteResult.Great => greatSprite,
                NoteResult.Ok => okSprite,
                NoteResult.Miss => missSprite,
                _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
            };
            
            StopCoroutine(nameof(Pulse));
            StartCoroutine(nameof(Pulse));
        }

        private IEnumerator Pulse()
        {
            timeToLive = 0;
            timeToPulse = 0;
            transform.localScale = Vector3.one;
            
            while (timeToLive < duration)
            {
                timeToLive += Time.deltaTime;
                timeToPulse += Time.deltaTime;
                float scale;
                scale = timeToPulse < pulseDuration 
                    ? Mathf.Lerp(1, scaleMax, timeToPulse / pulseDuration) 
                    : Mathf.Lerp(scaleMax, 1, (timeToLive - pulseDuration) / (duration - pulseDuration));
                
                transform.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }
            
            gameObject.SetActive(false);
        }
    }
}