using System.Collections.Generic;
using LevelSystem.Config;
using UniRx;
using UnityEngine;

namespace LevelSystem.Model
{
    public interface IGameplayModel
    {
        IReadOnlyReactiveProperty<Vector2Int> FoodPosition { get; }
        void SpawnFood(List<Vector2Int> occupiedPositions);
    }
}