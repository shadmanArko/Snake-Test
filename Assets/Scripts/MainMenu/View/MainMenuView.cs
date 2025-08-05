using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainMenu.View
{
    public class MainMenuView : MonoBehaviour
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
        public List<MainMenuPage> MainMenuPages => _mainMenuPages;
    }
}