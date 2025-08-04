using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace LevelSystem.Model
{
    public interface IFoodModel
    {
        IReadOnlyReactiveProperty<Vector2Int> FoodPosition { get; }
        void SpawnFood(List<Vector2Int> occupiedPositions);
        Sprite GetVto();
    }
}