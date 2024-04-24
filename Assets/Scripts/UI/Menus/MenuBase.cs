using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Menus
{
    public abstract class MenuBase : MonoBehaviour
    {
        [SerializeField] private GameObject display;
        [SerializeField] private List<Page> pages;
        
        private int currentPage;
        protected virtual bool StartOpen => false;
        protected abstract Action OpenTrigger { get; set; }
        protected abstract Action CloseTrigger { get; set; }

        private void Awake()
        {
            if (display == null) display = transform.GetChild(0).gameObject;
            if (pages.Count == 0) pages.Add(new Page(display));
            pages.ForEach(x => x.ToggleDisplay(false));
        }
        
        private void JumpToStartPage() => JumpToPage(0);

        private void OnEnable()
        {
            if(StartOpen) 
                OpenMenu();
            else 
                CloseMenu();
            OpenTrigger += OpenMenu;
        }

        public virtual void OpenMenu()
        {
            display.SetActive(true);
            Controller.UIController.Instance.SwapToUI();
            if (pages.Count > 1) Controller.UIController.OnGoBack += JumpToStartPage;
            JumpToPage(0);
            CloseTrigger += CloseMenu;
        }

        public void JumpToPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= pages.Count)
            {
                Debug.LogError("Invalid page index");
                return;
            }

            pages[currentPage].ToggleDisplay(false);
            pages[pageIndex].ToggleDisplay(true);
            currentPage = pageIndex;
        }
    
        public void JumpToPage(GameObject page)
        {
            JumpToPage(pages.FindIndex(p => p.IsMatch(page)));
        }

        private void HideAllPages()
        {
            pages.ForEach(p => p.ToggleDisplay(false));
        }

        public virtual void CloseMenu()
        {
            display.SetActive(false);
            HideAllPages();
            Controller.UIController.Instance.SwapToGameplay();
            if (pages.Count > 1) Controller.UIController.OnGoBack -= JumpToStartPage;
            CloseTrigger -= CloseMenu;
        }

        private void OnDisable()
        {
            OpenTrigger -= OpenMenu;
            CloseTrigger -= CloseMenu;
        }

        private void OnDestroy()
        {
            Controller.UIController.OnGoBack -= JumpToStartPage;
        }
    }
}