using System;
using UnityEngine;

namespace UI.Menus
{
    public class GenericMenu : MenuBase
    {
        [SerializeField] private bool startsOpen;
        protected override bool StartOpen => startsOpen;
        protected override Action OpenTrigger { get ; set; }
        protected override Action CloseTrigger { get; set; }
    }
}