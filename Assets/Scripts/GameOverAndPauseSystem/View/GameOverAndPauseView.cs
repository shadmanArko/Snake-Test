using CodeMonkey.Utils;
using GameOverAndPauseSystem.Config;
using UnityEngine;

namespace GameOverAndPauseSystem.View
{
    public class GameOverAndPauseView : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private Button_UI restartButton;
        
        public GameObject GameOverUI => gameOverUI;
        public GameObject PauseUI => pauseUI;
        public Button_UI RestartButton => restartButton;

        public void ApplyVto(GameStateConfig config)
        {
            gameOverUI.SetActive(config.ShowGameOverUI);
            pauseUI.SetActive(config.ShowPauseUI);
        }
    }
}