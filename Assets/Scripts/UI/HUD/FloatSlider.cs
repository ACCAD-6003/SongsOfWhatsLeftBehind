using System.Collections;
using SharedData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class FloatSlider : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private SharedFloat trackedValue;

        [Header("Animation")] 
        [SerializeField] private bool shouldAnimate;
        [SerializeField] private Image delayedFillImage;
        [SerializeField] private float pulseDegree = 1.2f;

        private void OnEnable()
        {
            trackedValue.OnValueChanged += UpdateDisplay;
            InitializeDisplay();
        }

        private void InitializeDisplay()
        {
            shouldAnimate = false;
            UpdateDisplay();
            shouldAnimate = true;
            if(delayedFillImage != null) delayedFillImage.fillAmount = fillImage.fillAmount;
        }

        private void UpdateDisplay()
        {
            var min = trackedValue.MinValue;
            var max = trackedValue.MaxValue;
            var value = trackedValue.Value;
        
            fillImage.fillAmount = (value - min) / (max - min);
            if(!shouldAnimate) return;
            StopCoroutine(nameof(PlayAnimation));
            StartCoroutine(nameof(PlayAnimation));
        }

        private IEnumerator PlayAnimation()
        {
            gameObject.transform.localScale = Vector3.one * pulseDegree;
            yield return new WaitForSeconds(0.1f);
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

            if (delayedFillImage.fillAmount > fillImage.fillAmount)
            {
                yield return new WaitForSeconds(0.5f);
                while(delayedFillImage.fillAmount > fillImage.fillAmount)
                {
                    delayedFillImage.fillAmount -= 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            
            delayedFillImage.fillAmount = fillImage.fillAmount;
        }

        private void OnDisable()
        {
            trackedValue.OnValueChanged -= UpdateDisplay;
        }
    }
}
