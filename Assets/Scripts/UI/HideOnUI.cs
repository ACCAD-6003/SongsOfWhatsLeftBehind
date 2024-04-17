using System;
using UnityEngine;

namespace UI
{
    public class HideOnUI : MonoBehaviour
    {
        private void Awake()
        {
            Controller.UIController.Instance.OnSwapToUI += Hide;
            Controller.UIController.Instance.OnSwapToGameplay += Show;
            if (Controller.UIController.Instance.InGameplay) Show();
            else Hide();
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            Controller.UIController.Instance.OnSwapToUI -= Hide;
            Controller.UIController.Instance.OnSwapToGameplay -= Show;
        }
    }
}