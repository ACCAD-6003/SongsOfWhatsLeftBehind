using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class NoteImage : MonoBehaviour
    {
        [SerializeField] NoteType noteType;
        [SerializeField] private SpriteRenderer extendedNoteSprite;
        [SerializeField] private SpriteRenderer connector;
        
        public NoteType NoteType => noteType;
        public float Position => transform.position.y;
        public float ExtendedPosition => extendedNoteSprite.transform.position.y;
        
        private float startingHeight;
        private Func<float, bool> withinThreshold;
        private SpriteRenderer spriteRenderer;
        private NoteStyle style;
        private Action score;
        private Action loseScore;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Setup(NoteTemplate noteTemplate, NoteStyle noteStyle = NoteStyle.Single)
        {
            noteType = noteTemplate.noteType;
            style = noteStyle;
            loseScore = null;
            spriteRenderer.color =
                noteStyle == NoteStyle.Single ? noteTemplate.singleNoteIcon : noteTemplate.longNoteIcon;
            connector.gameObject.SetActive(noteStyle != NoteStyle.Single);
            extendedNoteSprite.gameObject.SetActive(noteStyle != NoteStyle.Single);
            extendedNoteSprite.color = spriteRenderer.color;
        }
        
        public void Send(float timeToReachBottom, float targetZone, Func<float, bool> withinThreshold, float length = 0)
        {
            startingHeight = Position;
            this.withinThreshold = withinThreshold;
            var speed = CalculateSpeed(timeToReachBottom, startingHeight, targetZone);
            SetPosition(extendedNoteSprite.transform, Position + length * -speed);
            SetupConnector(connector, ExtendedPosition - Position);
            
            StartCoroutine(MoveToBottom(speed));
        }

        public void Hit(Action score, Action loseScore)
        {
            switch (style)
            {
                case NoteStyle.Hold:
                    connector.color = Color.gray;
                    this.score = score;
                    this.loseScore = loseScore;
                    Controller.RhythmGameController.OnNoteReleasedProcessed -= UponRelease;
                    Controller.RhythmGameController.OnNoteReleasedProcessed += UponRelease;
                    break;
                case NoteStyle.Single:
                    this.loseScore = null;
                    gameObject.SetActive(false);
                    score();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UponRelease(NoteType type)
        {
            if ((noteType & type) == 0) return;

            if (withinThreshold(ExtendedPosition)) score();
            else loseScore();
            loseScore = null;
            gameObject.SetActive(false);
        }

        private static void SetPosition(Transform t, float height)
        {
            var temp = t.position;
            temp.y = height;
            t.position = temp;
        }

        private void SetupConnector(SpriteRenderer connector, float distance)
        {
            var connectorTransform = connector.transform;
            connectorTransform.localPosition = Vector3.up * distance / 2f;
            var temp = connectorTransform.localScale;
            temp.y = distance;
            connectorTransform.localScale = temp;
            connector.color = Color.white;
        }

        private IEnumerator MoveToBottom(float speed)
        {
            while (ExtendedPosition > -startingHeight)
            {
                SetPosition(transform, Position + speed * Time.deltaTime);
                yield return null;
            }
            gameObject.SetActive(false);
        }

        private static float CalculateSpeed(float time, float startingHeight, float endingHeight)
        {
            return (endingHeight - startingHeight) / time;
        }

        private void OnDisable()
        {
            loseScore?.Invoke();
            Controller.RhythmGameController.OnNoteReleasedProcessed -= UponRelease;
        }
    }
}