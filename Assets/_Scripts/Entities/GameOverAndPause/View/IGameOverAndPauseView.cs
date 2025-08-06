using _Scripts.Entities.GameOverAndPause.Config;
using _Scripts.Services.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Entities.GameOverAndPause.View
{
    public interface IGameOverAndPauseView
    {
        GameObject GameOverUI { get; }
        GameObject PauseUI { get; }
        Button_UI RestartButton { get; }
        GameObject NewHighScoreTitleTest { get; }
        Text ScoreText { get; }
        Text HighScoreText { get; }
        void ApplyVto(GameStateConfig config);
    }
}