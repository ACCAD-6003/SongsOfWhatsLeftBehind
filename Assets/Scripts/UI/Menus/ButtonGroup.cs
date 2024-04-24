using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Menus
{
    public class ButtonGroup : MonoBehaviour
    {
        List<IButton> buttons;
    
        public void OnEnable()
        {
            SetButton(0);
        }

        private void Awake()
        {
            buttons = GetComponentsInChildren<IButton>().ToList();
        }

        private void SetButton(int index)
        {
            Debug.Assert(index >= 0 && index < buttons.Count, "Invalid button index");
            buttons[index].ToggleSelected(true);
        }

        public void Refresh()
        {
            GetComponentInChildren<IButton>().ToggleSelected(true);
        }
    }
}
