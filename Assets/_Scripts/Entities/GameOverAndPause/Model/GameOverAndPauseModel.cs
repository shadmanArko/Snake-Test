using _Scripts.Entities.GameOverAndPause.Config;
using _Scripts.Services.Persistence;
using _Scripts.Services.Persistence.Models;
using UnityEngine;

namespace _Scripts.Entities.GameOverAndPause.Model
{
    public class GameOverAndPauseModel : IGameOverAndPauseModel
    {
        private const float PausedTimeScale = 0f;
        private const float ResumedTimeScale = 1f;
        
        private readonly IGameOverAndPauseConfig _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Level _level;

        public GameOverAndPauseModel(IGameOverAndPauseConfig config, IUnitOfWork unitOfWork)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _level = _unitOfWork.Levels.GetById(_config.GameLevelConfig.LevelId);
        }

        public GameStateConfig GameOver()
        {
            return _config.GameOverStateConfig;
        }

        public GameStateConfig Pause()
        {
            SetTimeScale(PausedTimeScale);
            return _config.PauseStateConfig;
        }

        public GameStateConfig Resume()
        {
            SetTimeScale(ResumedTimeScale);
            return _config.GameResumeConfig;
        }

        public bool ShowNewHighScoreTextTitle()
        {
            return IsNewHighScore();
        }

        public int GetScore()
        {
            return _level.score;
        }

        public int GetHighScore()
        {
            return _level.highestScore;
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }

        private void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

        private bool IsNewHighScore()
        {
            return _level.highestScore <= _level.score;
        }
    }
}