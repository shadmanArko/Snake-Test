using System;
using _Scripts.Entities.GameOverAndPause.Model;
using _Scripts.Entities.GameOverAndPause.View;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using UniRx;
using UniRx.Triggers;

namespace _Scripts.Entities.GameOverAndPause.Controller
{
    public class GameOverAndPauseController : IDisposable, IGameOverAndPauseController
    {
        private readonly IEventBus _eventBus;
        private readonly IGameOverAndPauseModel _model;
        private readonly GameOverAndPauseView _view;
        private readonly CompositeDisposable _disposables = new();

        public GameOverAndPauseController(IEventBus eventBus, IGameOverAndPauseModel model, GameOverAndPauseView view)
        {
            _eventBus = eventBus;
            _model = model;
            _view = view;

            _view.RestartButton.ClickFunc += () => _eventBus.Publish(new RestartGameSceneEvent());
            _eventBus.OnEvent<SnakeDiedEvent>()
                .TakeUntil(_view.gameObject.OnDestroyAsObservable())
                .Subscribe(_ =>
                {
                    _view.ApplyVto(_model.GameOver());
                    _view.NewHighScoreTitleTest.SetActive(_model.ShowNewHighScoreTextTitle());
                    _view.ScoreText.text = _model.GetScore().ToString();
                    _view.HighScoreText.text = _model.GetHighScore().ToString();
                });
        }

        public void Dispose()
        {
            _view.RestartButton.ClickFunc -= () => _eventBus.Publish(new RestartGameSceneEvent());
            _disposables.Dispose();
        }
    }
}