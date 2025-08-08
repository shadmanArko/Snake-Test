using System;
using System.Collections.Generic;
using _Scripts.Entities.Food.Model;
using _Scripts.Entities.Food.View;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Scripts.Entities.Food.Controller
{
    public class FoodController : IFoodController, IDisposable, IInitializable
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
        }

        public void Initialize()
        {
            BindModelToView();
            SubscribeToEvents();
            SetupModelAndView().Forget();
        }

        public void Dispose()
        {
        }

        private async UniTask SetupModelAndView()
        {
            await _model.LoadFoodSprite();
            _view.ApplyVto(_model.FoodSprite);
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
                .Subscribe(eventArgs => SpawnNewFood(eventArgs.OccupiedPositions))
                .AddTo(_disposables);
        }
        
        private void SpawnNewFood(List<Vector2Int> occupiedPositions)
        {
            _model.SpawnFood(occupiedPositions);
        }
    }
}