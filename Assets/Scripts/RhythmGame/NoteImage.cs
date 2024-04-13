using System;
using System.Collections;
using Unity.Mathematics;
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
        [SerializeField] private GameObject pop;

        public NoteType NoteType => noteType;
        public float Position => transform.position.y;
        public float HorizontalPosition => noteSprite.position.x;
        public float VerticalPosition => noteSprite.position.y;
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
        private RectTransform connectorTransform;
        private bool hasBeenHit;
        private Action onBottomReached;

        private void Awake()
        {
            spriteRenderer = noteSprite.GetComponent<Image>();
            connectorTransform = connector.GetComponent<RectTransform>();
        }

        public void Setup(NoteTemplate noteTemplate, Action onBottomReached, NoteStyle noteStyle = NoteStyle.Single)
        {
            noteType = noteTemplate.noteType;
            style = noteStyle;
            loseScore = null;
            spriteRenderer.sprite = noteTemplate.noteIcon;
            connector.gameObject.SetActive(noteStyle != NoteStyle.Single);
            extendedNoteSprite.gameObject.SetActive(noteStyle != NoteStyle.Single);
            extendedNoteSprite.sprite = spriteRenderer.sprite;
            this.noteTemplate = noteTemplate;
            hasBeenHit = false;
            this.onBottomReached = onBottomReached;
        }

        public void Send(float timeToReachBottom, float targetZone, Func<float, bool> withinThreshold, float length = 0)
        {
            startingHeight = Position;
            this.withinThreshold = withinThreshold;
            this.targetZone = targetZone;
            var speed = CalculateSpeed(timeToReachBottom, startingHeight, targetZone);
            extendedNoteSprite.transform.position = transform.position;
            SetupConnector(ExtendedPosition - Position);
            
            var currentX = transform.position.x;
            preMarginXSpeed = CalculateSpeed(timeToReachBottom, currentX, noteTemplate.marginX + currentX);

            postMarginXSpeed = 0;
            SetPosition(noteSprite, Position);
            SetXOffset(noteSprite, 0);

            StartCoroutine(HoldForDuration(length));
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
                    score();
                    Controller.RhythmGameController.OnNoteReleasedProcessed -= UponRelease;
                    Controller.RhythmGameController.OnNoteReleasedProcessed += UponRelease;
                    break;
                case NoteStyle.Single:
                    this.loseScore = null;
                    gameObject.SetActive(false);
                    score();
                    Instantiate(pop, new Vector3(noteSprite.position.x, targetZone), quaternion.identity, transform.parent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            hasBeenHit = true;
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

        private void SetupConnector(float distance)
        {
            connectorTransform.localPosition = Vector3.up * distance / 2f;
            connectorTransform.sizeDelta = new Vector2(connectorTransform.rect.width, distance);
            connector.color = Color.white;
        }
        
        private void AngleConnector()
        {
            var top = extendedNoteSprite.transform;
            var bottom = noteSprite.transform;
            var angle = Vector2.SignedAngle(Vector2.up, top.position - bottom.position);
            connectorTransform.localRotation = Quaternion.Euler(0, 0, angle);
            connectorTransform.localPosition = (top.localPosition + bottom.localPosition) / 2;
            connectorTransform.sizeDelta = new Vector2(connectorTransform.rect.width, Vector2.Distance(top.localPosition, bottom.localPosition));
        }

        // Should move the note to the bottom of the screen crossing through the notee margin x and then endpoint x
        // The margin x occurs at the targetZone position
        private IEnumerator MoveToBottom(float speed)
        {
            Func<float> noteSpriteXOffset = () => noteSprite.localPosition.x + preMarginXSpeed * Time.deltaTime;
            Func<float> noMoveSprite = () => noteSprite.localPosition.x;
            Func<float> extendedNoteXOffset = () => extendedNoteSprite.transform.localPosition.x + preMarginXSpeed * Time.deltaTime;
            Func<float> noMoveExtended = () => extendedNoteSprite.transform.localPosition.x;
            var thresholdPassed = false;

            while (ExtendedPosition > -startingHeight)
            {
                var spriteXOffset = Position > targetZone 
                    ? noteSpriteXOffset : noMoveSprite;
                var noteXOffset = ExtendedPosition < startingHeight && ExtendedPosition > targetZone 
                    ? extendedNoteXOffset : noMoveExtended;

                if (hasBeenHit && style == NoteStyle.Hold && Position < targetZone) SetPosition(noteSprite, targetZone);
                if (!thresholdPassed) thresholdPassed = ThresholdPassed();
                
                UpdatePosition(speed, spriteXOffset, noteXOffset);
                yield return null;
            }

            gameObject.SetActive(false);
        }
        
        private IEnumerator HoldForDuration(float duration)
        {
            float timeElapsed = 0;
            var startingPosition = extendedNoteSprite.transform.position;
            extendedNoteSprite.enabled = false;
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                extendedNoteSprite.transform.position = startingPosition;
                yield return null;
            }
            extendedNoteSprite.enabled = true;
        }

        private bool ThresholdPassed()
        {
            if (hasBeenHit) return false;
            if (withinThreshold(Position)) return false;
            if (Position > targetZone) return false;
            
            onBottomReached();
            return true;
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