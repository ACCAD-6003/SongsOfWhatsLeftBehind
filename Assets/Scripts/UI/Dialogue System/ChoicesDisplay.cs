using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Dialogue_System
{
    public class ChoicesDisplay : MonoBehaviour
    {
        [SerializeField] GameObject choiceTemplate;
        [SerializeField] float scaleFactor;

        List<TextMeshProUGUI> choicesText = new();
        List<RectTransform> choices = new();
        List<SimpleButton> choiceButtons = new();

        Action<int> OnClick;

        public void Display(List<string> validChoices, Action<int> OnClick)
        {
            foreach(var choiceOption in validChoices)
            {
                GameObject instance = Instantiate(choiceTemplate, transform);
                var textBox = instance.transform.GetComponentInChildren<TextMeshProUGUI>();
                textBox.text = choiceOption;
                var uiButton = instance.transform.GetComponent<SimpleButton>();
                uiButton.OnClick += UiButton_OnClick;
                uiButton.OnSelect += UiButton_OnSelect;
                choicesText.Add(textBox);
                choices.Add(instance.GetComponent<RectTransform>());
                choiceButtons.Add(uiButton);
            }

            this.OnClick = OnClick;
        }

        private void UiButton_OnClick(IButton obj)
        {
            OnClick(choices.Select(x => x.GetComponent<IButton>()).ToList().IndexOf(obj));
        }

        private void UiButton_OnSelect(IButton obj)
        {
            var choice = choices.Select(x => x.GetComponent<IButton>()).ToList().IndexOf(obj);
        }

        public void SelectChoice(int index)
        {
            choiceButtons[index].ToggleSelected(true);
        }

        public void Hide()
        {
            DestroyChildren();
        }

        private void DestroyChildren()
        {
            int children = transform.childCount - 1;
            while(children >= 0)
            {
                choiceButtons[children].OnSelect -= UiButton_OnClick;
                choiceButtons[children].OnSelect -= UiButton_OnSelect;
                Destroy(transform.GetChild(children).gameObject);
                children--;
            }
            choicesText.Clear();
            choices.Clear();
            choiceButtons.Clear();
        }
    }
}
