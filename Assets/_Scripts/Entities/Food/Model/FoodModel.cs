using System;
using System.Collections.Generic;
using _Scripts.EventBus;
using Cysharp.Threading.Tasks;
using HelperClasses;
using LevelSystem.Config;
using LevelSystem.Events;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelSystem.Model
{
    public class FoodModel : IFoodModel, IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ReactiveProperty<Vector2Int> _foodPosition = new ReactiveProperty<Vector2Int>();
        private readonly FoodConfig _config;
        
        private Sprite _foodSprite;
        public IReadOnlyReactiveProperty<Vector2Int> FoodPosition => _foodPosition;

        public FoodModel(IEventBus eventBus, FoodConfig config)
        {
            _eventBus = eventBus;
            _config = config;
        }
        
        public void SpawnFood(List<Vector2Int> occupiedPositions)
        {
            Vector2Int newFoodPosition;
            int attempts = 0;
            const int maxAttempts = 1000; // Prevent infinite loop

            do
            {
                newFoodPosition = new Vector2Int(
                    Random.Range(0, _config.gridConfig.width),
                    Random.Range(0, _config.gridConfig.height)
                );
                attempts++;
            } while (occupiedPositions.Contains(newFoodPosition) && attempts < maxAttempts);

            _foodPosition.Value = newFoodPosition;
            _eventBus.Publish(new FoodSpawnedEvent { Position = newFoodPosition });
        }
        
        public async UniTask<Sprite> GetVtoAsync()
        {
            _foodSprite = await AddressableHelper.LoadSpriteAsync(_config.foodSpriteAddressableKey);
            return _foodSprite;
        }
        
        public void Dispose()
        {
            AddressableHelper.ReleaseAsset(_foodSprite);
            _disposables?.Dispose();
            _foodPosition?.Dispose();
        }
    }
}