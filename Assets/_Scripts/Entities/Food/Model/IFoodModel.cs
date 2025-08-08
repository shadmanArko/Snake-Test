using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace _Scripts.Entities.Food.Model
{
    public interface IFoodModel
    {
        IReadOnlyReactiveProperty<Vector2Int> FoodPosition { get; }
        void SpawnFood(List<Vector2Int> occupiedPositions);
        Sprite FoodSprite { get; }
        UniTask LoadFoodSprite();
    }
}