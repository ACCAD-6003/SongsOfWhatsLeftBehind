using UnityEngine;

namespace UI.Dialogue_System
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogueAudioCueHandler : MonoBehaviour
    {
        [SerializeField] AudioDatabase audioDatabase;
        
        private AudioSource audioSource;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            DialogueManager.OnAudioCue += PlayAudioCue;
        }

        public void PlayAudioCue(string audioCue)
        {
            if (!audioDatabase.AudioClips.TryGetValue(audioCue, out var clip)) return;
            audioSource.PlayOneShot(clip);
        }
        
        private void OnDisable()
        {
            DialogueManager.OnAudioCue -= PlayAudioCue;
        }
    }
}