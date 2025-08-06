using System;
using _Scripts.Entities.Snake.Factory;
using _Scripts.Entities.Snake.Model;
using _Scripts.Entities.Snake.View;
using _Scripts.Enums;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using _Scripts.Services.InputSystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Scripts.Entities.Snake.Controller
{
    public class SnakeController : ISnakeController, IDisposable, IInitializable
    {
        private readonly ISnakeModel _model;
        private readonly ISnakeView _view;
        private readonly IEventBus _eventBus;
        private readonly ISnakeBodyPartFactory _bodyPartFactory;
        private readonly IGameInput _gameInput;
        private readonly CompositeDisposable _disposables;

        private IDisposable _moveTimer;

        public SnakeController(ISnakeModel model, ISnakeView view, IEventBus eventBus,
            ISnakeBodyPartFactory bodyPartFactory, IGameInput gameInput, CompositeDisposable disposables)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;
            _bodyPartFactory = bodyPartFactory;
            _gameInput = gameInput;
            _disposables = disposables;
        }

        public void Initialize()
        {
            BindModelToView();
            SetupInputHandling();
            
           _eventBus.OnEvent<SnakeDiedEvent>()
                .Subscribe(_ =>
                {
                    _moveTimer?.Dispose();
                })
                .AddTo(_disposables);
           
           _eventBus.OnEvent<FoodSpawnedEvent>()
               .Subscribe( e => _model.FoodPosition = e.Position)
               .AddTo(_disposables);
            
            _model.OnDirectionInput
                .Subscribe(direction => _model.SetDirection(direction))
                .AddTo(_disposables);
        }

        private void BindModelToView()
        {
            _model.HeadPosition
                .Subscribe(position => _view.SetHeadPosition(position))
                .AddTo(_disposables);
            
            _model.CurrentDirection
                .Subscribe(direction => _view.SetHeadRotation(_model.GetAngleFromDirection(direction)))
                .AddTo(_disposables);
            
            _model.BodyPositions
                .Subscribe(positions => _view.UpdateBodyParts(positions, () => _bodyPartFactory.CreateBodyPart()))
                .AddTo(_disposables);
        }

        private void SetupInputHandling()
        {
            // Subscribe to the input system's direction changes
            _gameInput.DirectionInput
                .Where(direction => direction.HasValue) // Only process when we have a valid direction
                .Select(direction => direction.Value) // Extract the direction value
                .Where(IsValidDirectionChange) // Prevent snake from going backwards into itself
                .Subscribe(direction => _model.DirectionInputSubject.OnNext(direction))
                .AddTo(_disposables);
        }

        private bool IsValidDirectionChange(Direction newDirection)
        {
            // Get current direction from model (you might need to expose this property)
            var currentDirection = _model.CurrentDirection.Value;
            
            // Prevent immediate reverse direction (snake can't go backwards into itself)
            switch (currentDirection)
            {
                case Direction.Up:
                    return newDirection != Direction.Down;
                case Direction.Down:
                    return newDirection != Direction.Up;
                case Direction.Left:
                    return newDirection != Direction.Right;
                case Direction.Right:
                    return newDirection != Direction.Left;
                default:
                    return true;
            }
        }
        
        public void Dispose()
        {
            _moveTimer?.Dispose();
        }
    }
}