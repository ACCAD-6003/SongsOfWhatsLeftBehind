using System;
using System.Collections;
using UnityEngine;

namespace RhythmGame
{
    public class NoteImage : MonoBehaviour
    {
        [SerializeField] NoteType noteType;
        
        public NoteType NoteType => noteType;
        
        private float startingHeight;

        public void Send(float timeToReachBottom, float targetZone)
        {
            startingHeight = transform.position.y;
            StartCoroutine(MoveToBottom(timeToReachBottom, targetZone));
        }
        
        private IEnumerator MoveToBottom(float timeToReachBottom, float targetZone)
        {
            float timeElapsed = 0;
            float speed = -(startingHeight - targetZone) / timeToReachBottom;
            while (transform.position.y > -startingHeight)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime);
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}