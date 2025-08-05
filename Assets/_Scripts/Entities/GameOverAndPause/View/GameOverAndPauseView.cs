using _Scripts.Entities.GameOverAndPause.Config;
using _Scripts.Services.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Entities.GameOverAndPause.View
{
    public class GameOverAndPauseView : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private Button_UI restartButton;
        [SerializeField] private GameObject newHighScoreTitleTest;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text highScoreText;
        
        public GameObject GameOverUI => gameOverUI;
        public GameObject PauseUI => pauseUI;
        public Button_UI RestartButton => restartButton;
        public GameObject NewHighScoreTitleTest => newHighScoreTitleTest;
        public Text ScoreText => scoreText;
        public Text HighScoreText => highScoreText;

        public void ApplyVto(GameStateConfig config)
        {
            gameOverUI.SetActive(config.ShowGameOverUI);
            pauseUI.SetActive(config.ShowPauseUI);
        }
    }
}