using System;
using _Scripts.Entities.Score.Config;
using _Scripts.GlobalConfigs;
using _Scripts.Services.Persistence.Models;
using _Scripts.Services.Persistence;
using UniRx;

namespace _Scripts.Entities.Score.Model
{
    public class ScoreModel : IScoreModel, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScoreConfig _config;
        private readonly CompositeDisposable _disposables;
        private readonly GameConfig _gameConfig;
        private readonly Level _level;
        
        private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<int> _highScore = new ReactiveProperty<int>(0);
        
        public IReadOnlyReactiveProperty<int> Score => _score;
        public IReadOnlyReactiveProperty<int> HighScore => _highScore;

        public ScoreModel(IUnitOfWork unitOfWork, IScoreConfig config, CompositeDisposable disposables, GameConfig gameConfig)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _disposables = disposables;
            _gameConfig = gameConfig;
            
            _level = _unitOfWork.Levels.GetById(_config.GameLevelConfig.LevelId);
            InitializeHighScore();
        }

        public void UpdateScore()
        {
            IncrementScore();
            UpdateHighScoreIfNeeded();
            SaveCurrentScore();
        }

        public void Dispose()
        {
            _unitOfWork.Save();
            _score?.Dispose();
            _highScore?.Dispose();
        }

        private void InitializeHighScore()
        {
            _highScore.Value = _level.highestScore;
        }

        private void IncrementScore()
        {
            _score.Value += _gameConfig.scorePerFood;
        }

        private void UpdateHighScoreIfNeeded()
        {
            if (_score.Value > _level.highestScore)
            {
                _level.highestScore = _score.Value;
                _highScore.Value = _level.highestScore;
            }
        }

        private void SaveCurrentScore()
        {
            _level.score = _score.Value;
        }
    }
}