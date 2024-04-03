using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    public class NoteImage : MonoBehaviour
    {
        [SerializeField] NoteType noteType;
        [SerializeField] private RectTransform noteSprite;
        [SerializeField] private Image extendedNoteSprite;
        [SerializeField] private Image connector;

        public NoteType NoteType => noteType;
        public float Position => transform.position.y;
        public float ExtendedPosition => extendedNoteSprite.transform.position.y;
        
        private float startingHeight;
        private Func<float, bool> withinThreshold;
        private NoteTemplate noteTemplate;
        private float targetZone;
        private float preMarginXSpeed;
        private float postMarginXSpeed;
        private Image spriteRenderer;
        private NoteStyle style;
        private Action score;
        private Action loseScore;

        private void Awake()
        {
            spriteRenderer = noteSprite.GetComponent<Image>();
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
            this.noteTemplate = noteTemplate;
        }
        
        public void Send(float timeToReachBottom, float targetZone, Func<float, bool> withinThreshold, float length = 0)
        {
            startingHeight = Position;
            this.withinThreshold = withinThreshold;
            this.targetZone = targetZone;
            var speed = CalculateSpeed(timeToReachBottom, startingHeight, targetZone);
            SetPosition(extendedNoteSprite.transform, Position + length * -speed);
            SetupConnector(connector, ExtendedPosition - Position);
            
            var currentX = transform.position.x;
            preMarginXSpeed = CalculateSpeed(timeToReachBottom, currentX, noteTemplate.marginX + currentX);

            postMarginXSpeed = 0;
            SetXOffset(noteSprite, 0);
            SetXOffset(extendedNoteSprite.transform, 0);

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
        
        private static void SetXOffset(Transform t, float width)
        {
            var temp = t.localPosition;
            temp.x = width;
            t.localPosition = temp;
        }

        private void SetupConnector(Image connector, float distance)
        {
            var connectorTransform = connector.GetComponent<RectTransform>();
            connectorTransform.localPosition = Vector3.up * distance / 2f;
            connectorTransform.sizeDelta = new Vector2(connectorTransform.rect.width, distance);
            connector.color = Color.white;
        }
        
        private void AngleConnector()
        {
            var top = extendedNoteSprite.transform;
            var bottom = noteSprite.transform;
            var angle = Vector2.SignedAngle(Vector2.up, top.position - bottom.position);
            connector.transform.localRotation = Quaternion.Euler(0, 0, angle);
            connector.transform.localPosition = (top.localPosition + bottom.localPosition) / 2;
        }

        // Should move the note to the bottom of the screen crossing through the notee margin x and then endpoint x
        // The margin x occurs at the targetZone position
        private IEnumerator MoveToBottom(float speed)
        {
            Func<float> noteSpriteXOffset = () => noteSprite.localPosition.x + preMarginXSpeed * Time.deltaTime;
            Func<float> noMoveSprite = () => noteSprite.localPosition.x;
            Func<float> extendedNoteXOffset = () => extendedNoteSprite.transform.localPosition.x + preMarginXSpeed * Time.deltaTime;
            Func<float> noMoveExtended = () => extendedNoteSprite.transform.localPosition.x;

            while (ExtendedPosition > -startingHeight)
            {
                var spriteXOffset = Position > targetZone 
                    ? noteSpriteXOffset : noMoveSprite;
                var noteXOffset = ExtendedPosition < startingHeight && ExtendedPosition > targetZone 
                    ? extendedNoteXOffset : noMoveExtended;

                UpdatePosition(speed, spriteXOffset, noteXOffset);
                yield return null;
            }
            
            gameObject.SetActive(false);
        }

        private void UpdatePosition(float speed, Func<float> noteSpriteXOffset, Func<float> extendedNoteXOffset)
        {
            var newY = Position + speed * Time.deltaTime;
            SetPosition(transform, newY);
            SetXOffset(noteSprite.transform, noteSpriteXOffset());
            SetXOffset(extendedNoteSprite.transform, extendedNoteXOffset());
            AngleConnector();
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