using System;
using System.Collections.Generic;
using _Scripts.Entities.Food.Model;
using _Scripts.Entities.Food.View;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace _Scripts.Entities.Food.Controller
{
    public class FoodController : IFoodController, IDisposable
    {
        private readonly IFoodModel _model;
        private readonly IFoodView _view;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables;

        public FoodController(
            IFoodModel model, 
            IFoodView view, 
            IEventBus eventBus, 
            CompositeDisposable disposables)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;
            _disposables = disposables;
            Initialize();
        }

        public void Initialize()
        {
            SetupView();
            
            BindModelToView();
            
            SubscribeToEvents();
        }

        private void SetupView()
        {
            LoadAndApplySprite().Forget();
        }

        private async UniTaskVoid LoadAndApplySprite()
        {
            Sprite sprite = await _model.GetVtoAsync();
            _view.ApplyVto(sprite);
        }


        private void BindModelToView()
        {
            _model.FoodPosition
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
        }
    }
}