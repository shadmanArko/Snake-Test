using System;
using _Scripts.Entities.Snake.Factory;
using _Scripts.Entities.Snake.Model;
using _Scripts.Entities.Snake.View;
using _Scripts.Enums;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
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
        private readonly CompositeDisposable _disposables;

        private IDisposable _moveTimer;

        public SnakeController(ISnakeModel model, ISnakeView view, IEventBus eventBus,
            ISnakeBodyPartFactory bodyPartFactory, CompositeDisposable disposables)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;
            _bodyPartFactory = bodyPartFactory;
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
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.UpArrow))
                .Subscribe(_ => _model.DirectionInputSubject.OnNext(Direction.Up))
                .AddTo(_disposables);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.DownArrow))
                .Subscribe(_ => _model.DirectionInputSubject.OnNext(Direction.Down))
                .AddTo(_disposables);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.LeftArrow))
                .Subscribe(_ => _model.DirectionInputSubject.OnNext(Direction.Left))
                .AddTo(_disposables);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.RightArrow))
                .Subscribe(_ => _model.DirectionInputSubject.OnNext(Direction.Right))
                .AddTo(_disposables);
        }
        
        public void Dispose()
        {
            _moveTimer?.Dispose();
        }
    }
}