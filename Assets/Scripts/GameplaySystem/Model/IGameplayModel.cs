using System.Collections.Generic;
using LevelSystem.Config;
using UniRx;
using UnityEngine;

namespace LevelSystem.Model
{
    public interface IGameplayModel
    {
        IReadOnlyReactiveProperty<Vector2Int> FoodPosition { get; }
        IReadOnlyReactiveProperty<int> Score { get; }
        
        bool TryEatFood(Vector2Int position);
        Vector2Int ValidateGridPosition(Vector2Int position);
        void SpawnFood(List<Vector2Int> occupiedPositions);
    }
}