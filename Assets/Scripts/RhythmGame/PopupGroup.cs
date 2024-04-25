using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    [CreateAssetMenu(fileName = "New Popup Group", menuName = "Tutorial/Popup Group")]
    public class PopupGroup : ScriptableObject
    {
        public List<PopupData> popups;
    }
}