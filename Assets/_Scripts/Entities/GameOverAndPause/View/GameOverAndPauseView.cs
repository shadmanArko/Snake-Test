using _Scripts.Entities.GameOverAndPause.Config;
using _Scripts.Services.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Entities.GameOverAndPause.View
{
    public class GameOverAndPauseView : MonoBehaviour, IGameOverAndPauseView
    {
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private Button_UI restartButton;
        [SerializeField] private Button_UI pauseButton;
        [SerializeField] private Button_UI resumeButton;
        [SerializeField] private Button_UI quitButton;
        [SerializeField] private GameObject newHighScoreTitleTest;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text highScoreText;
        
        public GameObject GameOverUI => gameOverUI;
        public GameObject PauseUI => pauseUI;
        public Button_UI RestartButton => restartButton;
        public Button_UI PauseButton => pauseButton;
        public Button_UI ResumeButton => resumeButton;
        public Button_UI QuitButton => quitButton;
        public GameObject NewHighScoreTitleTest => newHighScoreTitleTest;
        public Text ScoreText => scoreText;
        public Text HighScoreText => highScoreText;

        public void ApplyVto(GameStateConfig config)
        {
            GameOverUI.SetActive(config.ShowGameOverUI);
            PauseUI.SetActive(config.ShowPauseUI);
            pauseButton.gameObject.SetActive(!config.ShowPauseUI);
        }
    }
}