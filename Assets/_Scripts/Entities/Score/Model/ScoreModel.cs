using System;
using _Scripts.Entities.Score.Config;
using _Scripts.Services.Persistence.Models;
using _Scripts.Services.Persistence;
using UniRx;

namespace _Scripts.Entities.Score.Model
{
    public class ScoreModel : IScoreModel, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<int> _highScore = new ReactiveProperty<int>(0);
        private readonly UnitOfWork _unitOfWork;
        private readonly ScoreConfig _config;
        
        private Level _level;
        
        public IReadOnlyReactiveProperty<int> Score => _score;
        public IReadOnlyReactiveProperty<int> HighScore => _highScore;

        public ScoreModel(UnitOfWork unitOfWork, ScoreConfig config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _level = _unitOfWork.Levels.GetById(_config.levelId);
            _highScore.Value = _level.highestScore;
        }

        public void UpdateScore()
        {
            _score.Value += _config.scorePerFood;
            
            if (_score.Value > _level.highestScore)
            {
                _level.highestScore = _score.Value;
                _highScore.Value = _level.highestScore;
            }
            _level.score = _score.Value;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _score?.Dispose();
            _highScore?.Dispose();
        }
    }
}