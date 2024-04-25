using System;
using System.Collections;
using Controller;
using TMPro;
using UnityEngine;

namespace UI.Remapping
{
    public class RemappingButton : UIButton
    {
        [SerializeField] private string keyName;
        [SerializeField] private TMP_Text currentKeyText;
        [SerializeField] private TMP_Text keyNameText;
        
        private UIController uiController;
        
        public override void Awake()
        {
            base.Awake();
            uiController = UIController.Instance;
            keyNameText.text = keyName;
        }

        private void OnEnable()
        {
            ReplaceKey();
        }

        private void ReplaceKey()
        {
            var keyText = uiController.GetKey(keyName);
            keyText = keyText.Replace("D-Pad/", "");
            currentKeyText.text = keyText;
        }

        public override void Use()
        {
            StartCoroutine(HandleKeyRemapping());
        }
        
        private IEnumerator HandleKeyRemapping()
        {
            currentKeyText.text = "Press a key";
            yield return uiController.AllowUserToSetKey(keyName);
            ReplaceKey();
        }
    }
}