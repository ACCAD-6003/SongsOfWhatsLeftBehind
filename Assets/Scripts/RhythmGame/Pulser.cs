using System.Collections;
using UnityEngine;

namespace RhythmGame
{
    public class Pulser : MonoBehaviour
    {
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private float pulseDuration = .05f;
        [SerializeField] private float scaleMax = 1.05f;

        private RhythmGameManager rhythmGame;

        private void Awake()
        {
            rhythmGame = FindObjectOfType<RhythmGameManager>();
        }

        private void OnEnable()
        {
            rhythmGame.OnHit += TriggerPulse;
        }

        public void TriggerPulse()
        {
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
                float scale;
                scale = timeToPulse < pulseDuration 
                    ? Mathf.Lerp(1, scaleMax, timeToPulse / pulseDuration) 
                    : Mathf.Lerp(scaleMax, 1, (timeToLive - pulseDuration) / (duration - pulseDuration));
                
                transform.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }
        }
        
        private void OnDisable()
        {
            rhythmGame.OnHit -= TriggerPulse;
        }
    }
}