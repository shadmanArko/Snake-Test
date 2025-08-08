using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Scripts.Events;
using _Scripts.GlobalConfigs;
using _Scripts.Services.EventBus.Core;
using _Scripts.HelperClasses;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Entities.Food.Model
{
    public class FoodModel : IFoodModel, IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables;
        private readonly ReactiveProperty<Vector2Int> _foodPosition = new ReactiveProperty<Vector2Int>();
        private readonly GameConfig _config;
        
        private Sprite _foodSprite;
        public IReadOnlyReactiveProperty<Vector2Int> FoodPosition => _foodPosition;
        public Sprite FoodSprite => _foodSprite;

        public FoodModel(IEventBus eventBus, GameConfig config, CompositeDisposable disposables)
        {
            _eventBus = eventBus;
            _config = config;
            _disposables = disposables;
        }
        
        
        public void SpawnFood(List<Vector2Int> occupiedPositions)
        {
            Vector2Int newFoodPosition;
            int attempts = 0;

            do
            {
                newFoodPosition = new Vector2Int(
                    Random.Range(0, _config.gridWidth),
                    Random.Range(0, _config.gridHeight)
                );
                attempts++;
            } while (occupiedPositions.Contains(newFoodPosition) && attempts < _config.maxFoodSpawnAttempts);

            _foodPosition.Value = newFoodPosition;
            _eventBus.Publish(new FoodSpawnedEvent { Position = newFoodPosition });
        }

        public async UniTask LoadFoodSprite()
        {
            try
            {
                _foodSprite = await AddressableHelper.LoadSpriteAsync(_config.foodSpriteAddressableKey);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public void Dispose()
        {
            AddressableHelper.ReleaseAsset(_foodSprite);
            _foodPosition?.Dispose();
        }
    }
}