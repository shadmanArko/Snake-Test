using System;
using _Scripts.EventBus;
using LevelSystem.Events;
using ScoreSystem.Model;
using ScoreSystem.View;
using UniRx;
using UniRx.Triggers;

namespace ScoreSystem.Controller
{
    public class ScoreController : IScoreController, IDisposable
    {
        private readonly IScoreModel _model;
        private readonly ScoreView _view;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ScoreController(IScoreModel model, ScoreView view, IEventBus eventBus)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;

            _model.Score
                .TakeUntil(_view.gameObject.OnDestroyAsObservable())
                .Subscribe(score => _view.ScoreText.text = score.ToString())
                .AddTo(_disposables);
            
            _model.HighScore
                .TakeUntil(_view.gameObject.OnDestroyAsObservable())
                .Subscribe(score => _view.HighScoreText.text = score.ToString())
                .AddTo(_disposables);
            
            _eventBus.OnEvent<FoodEatenEvent>()
                .TakeUntil(_view.gameObject.OnDestroyAsObservable())
                .Subscribe(_ => _model.UpdateScore())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}