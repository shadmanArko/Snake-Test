using System;
using _Scripts.EventBus;
using LevelSystem.Events;
using LevelSystem.Model;
using SnakeSystem;
using UniRx;
using UniRx.Triggers;

namespace LevelSystem.Controller
{
    public class GameplayController : IGameplayController, IDisposable
    {
        private readonly IGameplayModel _model;
        private readonly GameplayView _view;
        private readonly ISnakeModel _snakeModel;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public GameplayController(
            IGameplayModel model, 
            GameplayView view, 
            ISnakeModel snakeModel,
            IEventBus eventBus)
        {
            _model = model;
            _view = view;
            _snakeModel = snakeModel;
            _eventBus = eventBus;
            Initialize();
        }

        public void Initialize()
        {
            BindModelToView();
            
            SubscribeToEvents();
            
            SpawnInitialFood();
        }

        private void BindModelToView()
        {
            _model.FoodPosition
                .TakeUntil(_view.gameObject.OnDestroyAsObservable())
                .Subscribe(position => _view.SetFoodPosition(position))
                .AddTo(_disposables);
        }

        private void SubscribeToEvents()
        {
            // When food is eaten, spawn new food
            _eventBus.OnEvent<FoodEatenEvent>()
                .Subscribe(_ => SpawnNewFood())
                .AddTo(_disposables);
        }

        private void SpawnInitialFood()
        {
            var occupiedPositions = _snakeModel.GetAllOccupiedPositions();
            _model.SpawnFood(occupiedPositions);
        }

        private void SpawnNewFood()
        {
            var occupiedPositions = _snakeModel.GetAllOccupiedPositions();
            _model.SpawnFood(occupiedPositions);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}