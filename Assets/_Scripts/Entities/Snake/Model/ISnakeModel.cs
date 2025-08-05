using System;
using System.Collections.Generic;
using _Scripts.Entities.Snake.ValueObjects;
using _Scripts.Enums;
using UniRx;
using UnityEngine;

namespace _Scripts.Entities.Snake.Model
{
    public interface ISnakeModel
    {
        IReadOnlyReactiveProperty<SnakeState> State { get; }
        IReadOnlyReactiveProperty<Vector2Int> HeadPosition { get; }
        IReadOnlyReactiveProperty<Direction> CurrentDirection { get; }
        IReadOnlyReactiveProperty<IReadOnlyList<SnakeMovePosition>> BodyPositions { get; }
        
        void SetDirection(Direction direction);
        void Move();
        void EatFood();
        void Die();
        float GetAngleFromDirection(Direction direction);
        
        List<Vector2Int> GetAllOccupiedPositions();
        Subject<Direction> DirectionInputSubject { get; set; }
        IObservable<Direction> OnDirectionInput { get; }
        public Vector2Int FoodPosition{ get; set; }
    }
}