using System;
using _Scripts.Entities.Score.Model;
using _Scripts.Entities.Score.View;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace _Scripts.Entities.Score.Controller
{
    public class ScoreController : IScoreController, IDisposable, IInitializable
    {
        private readonly IScoreModel _model;
        private readonly IScoreView _view;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables;
        
        public ScoreController(IScoreModel model, IScoreView view, IEventBus eventBus, CompositeDisposable disposables)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;
            _disposables = disposables;
        }
        
        public void Initialize()
        {
            _model.Score
                .Subscribe(score => _view.ScoreText.text = score.ToString())
                .AddTo(_disposables);
            
            _model.HighScore
                .Subscribe(score => _view.HighScoreText.text = score.ToString())
                .AddTo(_disposables);
            
            _eventBus.OnEvent<FoodEatenEvent>()
                .Subscribe(_ => _model.UpdateScore())
                .AddTo(_disposables);
        }
       
        public void Dispose()
        {
        }
    }
}