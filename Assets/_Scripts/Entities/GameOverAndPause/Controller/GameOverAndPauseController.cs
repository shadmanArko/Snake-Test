using System;
using _Scripts.Entities.GameOverAndPause.Model;
using _Scripts.Entities.GameOverAndPause.View;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using UniRx;
using Zenject;

namespace _Scripts.Entities.GameOverAndPause.Controller
{
    public class GameOverAndPauseController : IDisposable, IGameOverAndPauseController, IInitializable
    {
        private readonly IEventBus _eventBus;
        private readonly IGameOverAndPauseModel _model;
        private readonly IGameOverAndPauseView _view;
        private readonly CompositeDisposable _disposables;

        public GameOverAndPauseController(IEventBus eventBus, IGameOverAndPauseModel model, IGameOverAndPauseView view, CompositeDisposable disposables)
        {
            _eventBus = eventBus;
            _model = model;
            _view = view;
            _disposables = disposables;
        }
        
        public void Initialize()
        {
            _view.RestartButton.ClickFunc += OnRestartButtonClicked;
            _eventBus.OnEvent<SnakeDiedEvent>()
                .Subscribe(_ =>
                {
                    _view.ApplyVto(_model.GameOver());
                    _view.NewHighScoreTitleTest.SetActive(_model.ShowNewHighScoreTextTitle());
                    _view.ScoreText.text = _model.GetScore().ToString();
                    _view.HighScoreText.text = _model.GetHighScore().ToString();
                })
                .AddTo(_disposables);
        }

        private void OnRestartButtonClicked()
        {
            _eventBus.Publish(new RestartGameSceneEvent());
        }

        public void Dispose()
        {
            _view.RestartButton.ClickFunc -= OnRestartButtonClicked;
            _disposables?.Dispose();
        }
    }
}