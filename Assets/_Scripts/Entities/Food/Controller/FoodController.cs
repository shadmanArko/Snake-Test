using System;
using System.Collections.Generic;
using _Scripts.Entities.Food.Model;
using _Scripts.Entities.Food.View;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace _Scripts.Entities.Food.Controller
{
    public class FoodController : IFoodController, IDisposable
    {
        private readonly IFoodModel _model;
        private readonly FoodView _view;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public FoodController(
            IFoodModel model, 
            FoodView view, 
            IEventBus eventBus)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;
            Initialize();
        }

        public void Initialize()
        {
            _ = _view.ApplyVto(_model.GetVtoAsync());
            
            BindModelToView();
            
            SubscribeToEvents();
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
            _eventBus.OnEvent<FoodEatenEvent>()
                .Subscribe(e => SpawnNewFood(e.OccupiedPositions))
                .AddTo(_disposables);
        }
        
        private void SpawnNewFood(List<Vector2Int> occupiedPositions)
        {
            _model.SpawnFood(occupiedPositions);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}