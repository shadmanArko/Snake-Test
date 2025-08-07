using System;
using System.Collections.Generic;
using _Scripts.Entities.Snake.ValueObjects;
using _Scripts.Enums;
using Cysharp.Threading.Tasks;
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
        
        public Sprite SnakeHeadSprite { get; }
        public Sprite SnakeBodySprite { get; }
        UniTask LoadSnakeHeadSprite();
        UniTask LoadSnakeBodySprite();
        List<Vector2Int> GetAllOccupiedPositions();
        Subject<Direction> DirectionInputSubject { get; set; }
        IObservable<Direction> OnDirectionInput { get; }
        bool IsValidDirectionChange(Direction newDirection);
        public Vector2Int FoodPosition{ get; set; }
        UniTask<Sprite> GetVtoAsync();
    }
}