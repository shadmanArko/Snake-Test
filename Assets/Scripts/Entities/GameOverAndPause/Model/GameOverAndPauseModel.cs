using System;
using _Scripts.EventBus;
using GameCode.Persistence.Models;
using GameOverAndPauseSystem.Config;
using UnityEngine;

namespace GameOverAndPauseSystem.Model
{
    public class GameOverAndPauseModel : IGameOverAndPauseModel
    {
        private readonly GameOverAndPauseConfig _config;
        private readonly UnitOfWork _unitOfWork;
        
        private Level _level;

        public GameOverAndPauseModel(GameOverAndPauseConfig config, UnitOfWork unitOfWork)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _level = _unitOfWork.Levels.GetById("level01");
        }

        public GameStateConfig GameOver()
        {
            return _config.GameOverStateConfig;
        }
        

        public void Pause()
        {
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            Time.timeScale = 1f;
        }

        public bool ShowNewHighScoreTextTitle()
        {
            return _level.highestScore <= _level.score;
        }

        public int GetScore()
        {
           return _level.score;
        }

        public int GetHighScore()
        {
            return _level.highestScore;
        }
    }
}