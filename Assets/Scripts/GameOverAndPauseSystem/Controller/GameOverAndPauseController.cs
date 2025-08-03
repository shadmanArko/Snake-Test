using System;
using _Scripts.EventBus;
using GameOverAndPauseSystem.Model;
using GameOverAndPauseSystem.View;
using SceneFlowManagementSystem.Events;
using SnakeSystem;
using UniRx;
using UniRx.Triggers;

namespace GameOverAndPauseSystem.Controller
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
                .Subscribe(_ => _view.ApplyVto(_model.GameOver()));
        }

        public void Dispose()
        {
            _view.RestartButton.ClickFunc -= () => _eventBus.Publish(new RestartGameSceneEvent());
            _disposables.Dispose();
        }
    }
}