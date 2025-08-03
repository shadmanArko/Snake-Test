using System;
using _Scripts.EventBus;
using UniRx;

namespace SnakeSystem
{
    public class SnakeController : ISnakeController, IDisposable
    {
        private readonly ISnakeModel _model;
        private readonly ISnakeView _view;
        private readonly IEventBus _eventBus;
        private readonly SnakeConfig _config;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private IDisposable _moveTimer;

        public SnakeController(ISnakeModel model, ISnakeView view, IEventBus eventBus, SnakeConfig config)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;
            _config = config;
            Initialize();
        }

        public void Initialize()
        {
            _model.Initialize(_config);

            // Bind Model -> View (One-way data flow)
            BindModelToView();

            // Bind View -> Model (Input handling)
            BindViewToModel();

            // Handle global events
            SubscribeToEvents();

            // Start movement timer
            StartMovementTimer();
        }

        private void BindModelToView()
        {
            // Head position changes -> Update view
            _model.HeadPosition
                .Subscribe(position => _view.SetHeadPosition(position))
                .AddTo(_disposables);

            // Head direction changes -> Update view rotation
            _model.CurrentDirection
                .Subscribe(direction => _view.SetHeadRotation(direction))
                .AddTo(_disposables);

            // Body positions change -> Update view body parts
            _model.BodyPositions
                .Subscribe(positions => _view.UpdateBodyParts(positions))
                .AddTo(_disposables);
        }

        private void BindViewToModel()
        {
            // View input -> Model direction change
            _view.OnDirectionInput
                .Subscribe(direction => _model.SetDirection(direction))
                .AddTo(_disposables);
        }

        private void SubscribeToEvents()
        {
            // Listen to global events if needed
            _eventBus.OnEvent<SnakeDiedEvent>()
                .Subscribe(evt =>
                {
                    // Handle snake death (stop timer, etc.)
                    _moveTimer?.Dispose();
                })
                .AddTo(_disposables);
        }

        private void StartMovementTimer()
        {
            _moveTimer?.Dispose();
            _moveTimer = Observable.Interval(TimeSpan.FromSeconds(_config.moveInterval))
                .Where(_ => _model.State.Value == SnakeState.Alive)
                .Subscribe(_ => _model.Move())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _moveTimer?.Dispose();
            _disposables?.Dispose();
        }
    }
}