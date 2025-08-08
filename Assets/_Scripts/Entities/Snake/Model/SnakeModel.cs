using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Entities.Snake.Config;
using _Scripts.Entities.Snake.ValueObjects;
using _Scripts.Enums;
using _Scripts.Events;
using _Scripts.GlobalConfigs;
using _Scripts.HelperClasses;
using _Scripts.Services.EventBus.Core;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace _Scripts.Entities.Snake.Model
{
    public class SnakeModel : ISnakeModel, IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables;
        private readonly ISnakeConfig _snakeConfig;
        private readonly GameConfig _gameConfig;
        
        private readonly ReactiveProperty<SnakeState> _state = new(SnakeState.Alive);
        private readonly ReactiveProperty<Vector2Int> _headPosition = new();
        private readonly ReactiveProperty<Direction> _currentDirection = new();
        private readonly ReactiveProperty<IReadOnlyList<SnakeMovePosition>> _bodyPositions = new(new List<SnakeMovePosition>());
        private readonly List<SnakeMovePosition> _moveHistory = new();

        private IDisposable _moveTimer;
        private int _bodySize;
        private Sprite _snakeHeadSprite;
        private Sprite _snakeBodySprite;

        public IReadOnlyReactiveProperty<SnakeState> State => _state;
        public IReadOnlyReactiveProperty<Vector2Int> HeadPosition => _headPosition;
        public IReadOnlyReactiveProperty<Direction> CurrentDirection => _currentDirection;
        public IReadOnlyReactiveProperty<IReadOnlyList<SnakeMovePosition>> BodyPositions => _bodyPositions;
        public Sprite SnakeHeadSprite => _snakeHeadSprite;
        public Sprite SnakeBodySprite => _snakeBodySprite;
        
        public Subject<Direction> DirectionInputSubject { get; set; } = new();
        public IObservable<Direction> OnDirectionInput => DirectionInputSubject;
        public Vector2Int FoodPosition { get; set; } = new();

        public SnakeModel(IEventBus eventBus, ISnakeConfig snakeConfig, CompositeDisposable disposables, GameConfig gameConfig)
        {
            _eventBus = eventBus;
            _snakeConfig = snakeConfig;
            _disposables = disposables;
            _gameConfig = gameConfig;
            
            InitializeSnake();
            StartMovementTimer();
        }
        
        public async UniTask LoadSnakeHeadSprite()
        {
            _snakeHeadSprite = await AddressableHelper.LoadSpriteAsync(_gameConfig.snakeHeadSpriteAddressableKey);
        }
        
        public async UniTask LoadSnakeBodySprite()
        {
            _snakeBodySprite = await AddressableHelper.LoadSpriteAsync(_gameConfig.snakeBodySpriteAddressableKey);
        }

        public void SetDirection(Direction direction)
        {
            if (_state.Value != SnakeState.Alive) return;
            if (IsOppositeDirection(direction, _currentDirection.Value)) return;

            _currentDirection.Value = direction;
        }

        public void Move()
        {
            if (_state.Value != SnakeState.Alive) return;

            UpdateMoveHistory();
            MoveHead();
            CheckFood();
            TrimMoveHistory();
            UpdateBodyPositions();
            CheckSelfCollision();
        }

        public void EatFood()
        {
            _bodySize++;
            _eventBus.Publish(new FoodEatenEvent
            {
                Position = _headPosition.Value,
                OccupiedPositions = GetAllOccupiedPositions()
            });

            if (_snakeConfig.EnableSounds)
            {
                _eventBus.Publish(new PlaySfxEvent { ClipName = SoundClipName.SnakeEat });
            }
        }

        public void Die()
        {
            _state.Value = SnakeState.Dead;
            _eventBus.Publish(new SnakeDiedEvent { Position = _headPosition.Value });

            if (_snakeConfig.EnableSounds)
            {
                _eventBus.Publish(new PlaySfxEvent { ClipName = SoundClipName.SnakeDie });
            }
        }

        public List<Vector2Int> GetAllOccupiedPositions()
        {
            var positions = new List<Vector2Int> { _headPosition.Value };
            positions.AddRange(_bodyPositions.Value.Select(bodyPosition => bodyPosition.GridPosition));
            return positions;
        }

        public bool IsValidDirectionChange(Direction newDirection)
        {
            var currentDirection = _currentDirection.Value;
            
            return currentDirection switch
            {
                Direction.Up => newDirection != Direction.Down,
                Direction.Down => newDirection != Direction.Up,
                Direction.Left => newDirection != Direction.Right,
                Direction.Right => newDirection != Direction.Left,
                _ => true
            };
        }
        
        public async UniTask<Sprite> GetVtoAsync()
        {
            return await AddressableHelper.LoadSpriteAsync(_gameConfig.snakeHeadSpriteAddressableKey);
        }

        public float GetAngleFromDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Right => 0f,
                Direction.Up => 90f,
                Direction.Left => 180f,
                Direction.Down => 270f,
                _ => 0f
            };
        }

        public void Dispose()
        {
            _moveTimer?.Dispose();
            AddressableHelper.ReleaseAsset(_snakeHeadSprite);
            AddressableHelper.ReleaseAsset(_snakeBodySprite);
            _state?.Dispose();
            _headPosition?.Dispose();
            _currentDirection?.Dispose();
            _bodyPositions?.Dispose();
            DirectionInputSubject?.Dispose();
        }

        private void InitializeSnake()
        {
            _headPosition.Value = _snakeConfig.StartPosition;
            _currentDirection.Value = _snakeConfig.StartDirection;
            _state.Value = SnakeState.Alive;
            _bodySize = 0;
            _moveHistory.Clear();
        }
        
        private void StartMovementTimer()
        {
            _moveTimer?.Dispose();
            _moveTimer = Observable.Interval(TimeSpan.FromSeconds(_gameConfig.snakeMoveInterval))
                .Where(_ => State.Value == SnakeState.Alive)
                .Subscribe(_ => Move())
                .AddTo(_disposables);
        }

        private void UpdateMoveHistory()
        {
            var previousDirection = _moveHistory.Count > 0 ? _moveHistory[0].CurrentDirection : _currentDirection.Value;
            var movePosition = new SnakeMovePosition(_headPosition.Value, _currentDirection.Value, previousDirection);
            _moveHistory.Insert(0, movePosition);
        }

        private void MoveHead()
        {
            var newPosition = _headPosition.Value + GetDirectionVector(_currentDirection.Value);
            newPosition = ValidateGridPosition(newPosition);
            _headPosition.Value = newPosition;
        }

        private void CheckFood()
        {
            if (FoodPosition == _headPosition.Value)
            {
                EatFood();
            }
        }

        private void TrimMoveHistory()
        {
            if (_moveHistory.Count > _bodySize)
            {
                _moveHistory.RemoveAt(_moveHistory.Count - 1);
            }
        }

        private void UpdateBodyPositions()
        {
            _bodyPositions.Value = _moveHistory.Take(_bodySize).ToList();
        }

        private void CheckSelfCollision()
        {
            if (_bodyPositions.Value.Any(bodyPosition => bodyPosition.GridPosition == _headPosition.Value))
            {
                Die();
            }
            else
            {
                _eventBus.Publish(new SnakeMoveEvent { Position = _headPosition.Value, Direction = _currentDirection.Value });
            }
        }

        private bool IsOppositeDirection(Direction newDirection, Direction currentDirection)
        {
            return (newDirection == Direction.Up && currentDirection == Direction.Down) ||
                   (newDirection == Direction.Down && currentDirection == Direction.Up) ||
                   (newDirection == Direction.Left && currentDirection == Direction.Right) ||
                   (newDirection == Direction.Right && currentDirection == Direction.Left);
        }

        private Vector2Int GetDirectionVector(Direction direction)
        {
            return direction switch
            {
                Direction.Right => new Vector2Int(1, 0),
                Direction.Left => new Vector2Int(-1, 0),
                Direction.Up => new Vector2Int(0, 1),
                Direction.Down => new Vector2Int(0, -1),
                _ => Vector2Int.zero
            };
        }

        private Vector2Int ValidateGridPosition(Vector2Int position)
        {
            var validatedPosition = position;

            if (validatedPosition.x < 0)
                validatedPosition.x = _gameConfig.gridWidth - 1;
            else if (validatedPosition.x >= _gameConfig.gridWidth)
                validatedPosition.x = 0;

            if (validatedPosition.y < 0)
                validatedPosition.y = _gameConfig.gridHeight - 1;
            else if (validatedPosition.y >= _gameConfig.gridHeight)
                validatedPosition.y = 0;

            return validatedPosition;
        }
    }
}