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
            currentKeyText.text = uiController.GetKey(keyName);
            keyNameText.text = keyName;
        }

        public override void Use()
        {
            StartCoroutine(HandleKeyRemapping());
        }
        
        private IEnumerator HandleKeyRemapping()
        {
            currentKeyText.text = "Press a key";
            yield return uiController.AllowUserToSetKey(keyName);
            currentKeyText.text = uiController.GetKey(keyName);
        }
    }
}