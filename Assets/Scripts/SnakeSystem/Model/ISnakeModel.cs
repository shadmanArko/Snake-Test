using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace SnakeSystem
{
    public interface ISnakeModel
    {
        IReadOnlyReactiveProperty<SnakeState> State { get; }
        IReadOnlyReactiveProperty<Vector2Int> HeadPosition { get; }
        IReadOnlyReactiveProperty<Direction> CurrentDirection { get; }
        IReadOnlyReactiveProperty<IReadOnlyList<SnakeMovePosition>> BodyPositions { get; }
    
        void Initialize(SnakeConfig config);
        void SetDirection(Direction direction);
        void Move();
        void EatFood();
        void Die();
        List<Vector2Int> GetAllOccupiedPositions();
    }
}