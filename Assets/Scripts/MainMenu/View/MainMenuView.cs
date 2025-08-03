using System;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainMenu.View
{
    public class MainMenuView : MonoBehaviour, IMainMenuView
    {
        [SerializeField] private Button_UI _playButton;
        [SerializeField] private Button_UI _howToPlayButton;
        [SerializeField] private Button_UI _quitButton;
        [SerializeField] private Button_UI _backButton;
        [SerializeField] private List<MainMenuPage> _mainMenuPages;
        
        public Button_UI PlayButton => _playButton;
        public Button_UI HowToPlayButton => _howToPlayButton;
        public Button_UI QuitButton => _quitButton;
        public Button_UI BackButton => _backButton;
        
        public void SetPageActive(MainMenuPageType pageType, bool isActive)
        {
            var page = GetPageByType(pageType);
            
            if (page?.gameObject != null)
            {
                page.gameObject.SetActive(isActive);
            }
        }

        public void SetAllPagesInactive()
        {
            foreach (var page in _mainMenuPages)
            {
                if (page?.gameObject != null)
                {
                    page.gameObject.SetActive(false);
                }
            }
        }
        
        private MainMenuPage GetPageByType(MainMenuPageType pageType)
        {
            return _mainMenuPages?.FirstOrDefault(page => page.type == pageType);
        }
    }

    [Serializable]
    public class MainMenuPage
    {
        public MainMenuPageType type;
        public GameObject gameObject;
    }

    public enum MainMenuPageType
    {
        Home,
        HowToPlay,
    }
}