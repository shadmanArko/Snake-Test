using System;
using System.Collections.Generic;
using _Scripts.EventBus;
using GameCode.Persistence.Models;
using LevelSystem.Config;
using LevelSystem.Events;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelSystem.Model
{
    public class GameplayModel : IGameplayModel, IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly ReactiveProperty<Vector2Int> _foodPosition = new ReactiveProperty<Vector2Int>();
        private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>(0);

        private readonly GameplayConfig _config;
        
        public IReadOnlyReactiveProperty<Vector2Int> FoodPosition => _foodPosition;
        public IReadOnlyReactiveProperty<int> Score => _score;

        public GameplayModel(IEventBus eventBus, GameplayConfig config)
        {
            _eventBus = eventBus;
            _config = config;
        }

        public bool TryEatFood(Vector2Int position)
        {
            if (position == _foodPosition.Value)
            {
                _eventBus.Publish(new FoodEatenEvent
                {
                    Position = position,
                });
                return true;
            }

            return false;
        }

        public Vector2Int ValidateGridPosition(Vector2Int position)
        {
            var validatedPosition = position;

            if (validatedPosition.x < 0)
                validatedPosition.x = _config.width - 1;
            else if (validatedPosition.x >= _config.width)
                validatedPosition.x = 0;

            if (validatedPosition.y < 0)
                validatedPosition.y = _config.height - 1;
            else if (validatedPosition.y >= _config.height)
                validatedPosition.y = 0;

            return validatedPosition;
        }

        public void SpawnFood(List<Vector2Int> occupiedPositions)
        {
            Vector2Int newFoodPosition;
            int attempts = 0;
            const int maxAttempts = 1000; // Prevent infinite loop

            do
            {
                newFoodPosition = new Vector2Int(
                    Random.Range(0, _config.width),
                    Random.Range(0, _config.height)
                );
                attempts++;
            } while (occupiedPositions.Contains(newFoodPosition) && attempts < maxAttempts);

            _foodPosition.Value = newFoodPosition;
            _eventBus.Publish(new FoodSpawnedEvent { Position = newFoodPosition });
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _foodPosition?.Dispose();
            _score?.Dispose();
        }
    }
}