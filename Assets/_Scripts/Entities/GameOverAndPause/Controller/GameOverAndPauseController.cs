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
            SetupButtonActions();
            SubscribeToEvents();
        }

        public void Dispose()
        {
            UnsubscribeButtonActions();
            _disposables?.Dispose();
        }

        private void SetupButtonActions()
        {
            _view.RestartButton.ClickFunc += OnRestartButtonClicked;
            _view.PauseButton.ClickFunc += OnPauseButtonClicked;
            _view.ResumeButton.ClickFunc += OnResumeButtonClicked;
            _view.QuitButton.ClickFunc += OnQuitButtonClicked;
        }

        private void UnsubscribeButtonActions()
        {
            if (_view?.RestartButton != null)
                _view.RestartButton.ClickFunc -= OnRestartButtonClicked;
            
            if (_view?.PauseButton != null)
                _view.PauseButton.ClickFunc -= OnPauseButtonClicked;
            
            if (_view?.ResumeButton != null)
                _view.ResumeButton.ClickFunc -= OnResumeButtonClicked;
            
            if (_view?.QuitButton != null)
                _view.QuitButton.ClickFunc -= OnQuitButtonClicked;
        }

        private void SubscribeToEvents()
        {
            _eventBus.OnEvent<SnakeDiedEvent>()
                .Subscribe(_ => HandleSnakeDied())
                .AddTo(_disposables);
            
            _eventBus.OnEvent<PauseGameEvent>()
                .Subscribe(_ => HandlePauseGame())
                .AddTo(_disposables);
            
            _eventBus.OnEvent<ResumeGameEvent>()
                .Subscribe(_ => HandleResumeGame())
                .AddTo(_disposables);
        }

        private void HandleSnakeDied()
        {
            _view.ApplyVto(_model.GameOver());
            _view.NewHighScoreTitleTest.SetActive(_model.ShowNewHighScoreTextTitle());
            _view.ScoreText.text = _model.GetScore().ToString();
            _view.HighScoreText.text = _model.GetHighScore().ToString();
        }

        private void HandlePauseGame()
        {
            _view.ApplyVto(_model.Pause());
        }

        private void HandleResumeGame()
        {
            _view.ApplyVto(_model.Resume());
        }

        private void OnRestartButtonClicked()
        {
            _eventBus.Publish(new RestartGameSceneEvent());
        }

        private void OnPauseButtonClicked()
        {
            _eventBus.Publish(new PauseGameEvent());
        }

        private void OnResumeButtonClicked()
        {
            _eventBus.Publish(new ResumeGameEvent());
        }

        private void OnQuitButtonClicked()
        {
            _model.QuitGame();
        }
    }
}