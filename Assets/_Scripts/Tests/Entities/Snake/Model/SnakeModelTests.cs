using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using UniRx;
using _Scripts.Entities.Snake.Config;
using _Scripts.Entities.Snake.Model;
using _Scripts.Services.EventBus.Core;
using _Scripts.GlobalConfigs;
using _Scripts.Enums;
using _Scripts.Events;
using _Scripts.Entities.Snake.ValueObjects;

namespace _Scripts.Tests.Entities.Snake.Model
{
    [TestFixture]
    public class SnakeModelTests
    {
        private MockEventBus _mockEventBus;
        private SnakeConfig _snakeConfig;
        private GameConfig _gameConfig;
        private SnakeModel _snakeModel;
        private CompositeDisposable _disposables;

        [SetUp]
        public void SetUp()
        {
            SetupGameConfig();
            SetupSnakeConfig();
            SetupDependencies();
            CreateSnakeModel();
        }

        [TearDown]
        public void TearDown()
        {
            CleanupSnakeModel();
            CleanupDependencies();
            CleanupConfigs();
        }

        #region Setup/Teardown Helpers

        private void SetupGameConfig()
        {
            _gameConfig = ScriptableObject.CreateInstance<GameConfig>();
            _gameConfig.gridWidth = 20;
            _gameConfig.gridHeight = 20;
            _gameConfig.snakeMoveInterval = 0.2f;
        }

        private void SetupSnakeConfig()
        {
            _snakeConfig = ScriptableObject.CreateInstance<SnakeConfig>();
            _snakeConfig.StartPosition = new Vector2Int(10, 10);
            _snakeConfig.StartDirection = Direction.Right;
            _snakeConfig.BodyPartSortingOrder = 1;
            _snakeConfig.EnableSounds = true;
        }

        private void SetupDependencies()
        {
            _disposables = new CompositeDisposable();
            _mockEventBus = new MockEventBus();
        }

        private void CreateSnakeModel()
        {
            _snakeModel = new SnakeModel(_mockEventBus, _snakeConfig, _disposables, _gameConfig);
        }

        private void CleanupSnakeModel()
        {
            _snakeModel?.Dispose();
        }

        private void CleanupDependencies()
        {
            _disposables?.Dispose();
        }

        private void CleanupConfigs()
        {
            if (_snakeConfig != null)
                UnityEngine.Object.DestroyImmediate(_snakeConfig);
            if (_gameConfig != null)
                UnityEngine.Object.DestroyImmediate(_gameConfig);
        }

        #endregion

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidDependencies_InitializesCorrectly()
        {
            Assert.IsNotNull(_snakeModel);
            Assert.IsNotNull(_snakeModel.State);
            Assert.IsNotNull(_snakeModel.HeadPosition);
            Assert.IsNotNull(_snakeModel.CurrentDirection);
            Assert.IsNotNull(_snakeModel.BodyPositions);
        }

        [Test]
        public void Constructor_InitializesWithCorrectStartValues()
        {
            Assert.AreEqual(SnakeState.Alive, _snakeModel.State.Value);
            Assert.AreEqual(_snakeConfig.StartPosition, _snakeModel.HeadPosition.Value);
            Assert.AreEqual(_snakeConfig.StartDirection, _snakeModel.CurrentDirection.Value);
            Assert.AreEqual(0, _snakeModel.BodyPositions.Value.Count);
        }

        [Test]
        public void Constructor_WithDifferentStartDirections_InitializesCorrectly()
        {
            var directions = new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

            foreach (var direction in directions)
            {
                var config = CreateCustomSnakeConfig(direction, new Vector2Int(5, 5));
                var disposables = new CompositeDisposable();
                var snakeModel = new SnakeModel(_mockEventBus, config, disposables, _gameConfig);

                Assert.AreEqual(direction, snakeModel.CurrentDirection.Value);

                snakeModel.Dispose();
                disposables.Dispose();
                UnityEngine.Object.DestroyImmediate(config);
            }
        }

        #endregion

        #region State Tests

        [Test]
        public void State_InitiallyAlive()
        {
            Assert.AreEqual(SnakeState.Alive, _snakeModel.State.Value);
        }

        [Test]
        public void Die_ChangesStateToDead()
        {
            _snakeModel.Die();
            
            Assert.AreEqual(SnakeState.Dead, _snakeModel.State.Value);
        }

        [Test]
        public void Die_PublishesSnakeDiedEvent()
        {
            var initialPosition = _snakeModel.HeadPosition.Value;
            _snakeModel.Die();

            var diedEvent = _mockEventBus.GetStructEvent<SnakeDiedEvent>();
            Assert.IsNotNull(diedEvent);
            Assert.AreEqual(initialPosition, diedEvent.Value.Position);
        }

        [Test]
        public void Die_WithSoundsEnabled_PublishesPlaySfxEvent()
        {
            _snakeModel.Die();

            var sfxEvent = _mockEventBus.GetStructEvent<PlaySfxEvent>();
            Assert.IsNotNull(sfxEvent);
            Assert.AreEqual(SoundClipName.SnakeDie, sfxEvent.Value.ClipName);
        }

        #endregion

        #region Movement Tests

        [Test]
        public void Move_WhenAlive_UpdatesHeadPosition()
        {
            var initialPosition = _snakeModel.HeadPosition.Value;
            
            _snakeModel.Move();
            
            Assert.AreNotEqual(initialPosition, _snakeModel.HeadPosition.Value);
        }

        [Test]
        public void Move_WhenDead_DoesNotUpdatePosition()
        {
            _snakeModel.Die();
            var deadPosition = _snakeModel.HeadPosition.Value;
            
            _snakeModel.Move();
            
            Assert.AreEqual(deadPosition, _snakeModel.HeadPosition.Value);
        }

        [Test]
        public void Move_RightDirection_MovesCorrectly()
        {
            var initialPosition = _snakeModel.HeadPosition.Value;
            
            _snakeModel.Move();
            
            Assert.AreEqual(initialPosition + Vector2Int.right, _snakeModel.HeadPosition.Value);
        }

        [Test]
        public void Move_UpDirection_MovesCorrectly()
        {
            _snakeModel.SetDirection(Direction.Up);
            var initialPosition = _snakeModel.HeadPosition.Value;
            
            _snakeModel.Move();
            
            Assert.AreEqual(initialPosition + Vector2Int.up, _snakeModel.HeadPosition.Value);
        }

        [Test]
        public void Move_PublishesSnakeMoveEvent()
        {
            _snakeModel.Move();

            var moveEvent = _mockEventBus.GetStructEvent<SnakeMoveEvent>();
            Assert.IsNotNull(moveEvent);
            Assert.AreEqual(_snakeModel.HeadPosition.Value, moveEvent.Value.Position);
            Assert.AreEqual(_snakeModel.CurrentDirection.Value, moveEvent.Value.Direction);
        }

        #endregion

        #region Direction Tests

        [Test]
        public void SetDirection_ValidDirection_ChangesDirection()
        {
            _snakeModel.SetDirection(Direction.Up);
            
            Assert.AreEqual(Direction.Up, _snakeModel.CurrentDirection.Value);
        }

        [Test]
        public void SetDirection_OppositeDirection_DoesNotChange()
        {
            _snakeModel.SetDirection(Direction.Left); // Opposite to Right
            
            Assert.AreEqual(Direction.Right, _snakeModel.CurrentDirection.Value);
        }

        [Test]
        public void SetDirection_WhenDead_DoesNotChange()
        {
            _snakeModel.Die();
            _snakeModel.SetDirection(Direction.Up);
            
            Assert.AreEqual(Direction.Right, _snakeModel.CurrentDirection.Value);
        }

        [Test]
        public void IsValidDirectionChange_ValidDirections_ReturnsTrue()
        {
            Assert.IsTrue(_snakeModel.IsValidDirectionChange(Direction.Up));
            Assert.IsTrue(_snakeModel.IsValidDirectionChange(Direction.Down));
        }

        [Test]
        public void IsValidDirectionChange_OppositeDirection_ReturnsFalse()
        {
            Assert.IsFalse(_snakeModel.IsValidDirectionChange(Direction.Left));
        }

        #endregion

        #region Food Tests

        [Test]
        public void EatFood_IncreasesBodySize()
        {
            var initialBodyCount = _snakeModel.BodyPositions.Value.Count;
            
            _snakeModel.EatFood();
            
            // Body size increases but positions update happens on next move
            _snakeModel.Move();
            _snakeModel.Move(); // Move twice to see the body
            
            Assert.Greater(_snakeModel.BodyPositions.Value.Count, initialBodyCount);
        }

        [Test]
        public void EatFood_PublishesFoodEatenEvent()
        {
            var headPosition = _snakeModel.HeadPosition.Value;
            
            _snakeModel.EatFood();

            var foodEatenEvent = _mockEventBus.GetStructEvent<FoodEatenEvent>();
            Assert.IsNotNull(foodEatenEvent);
            Assert.AreEqual(headPosition, foodEatenEvent.Value.Position);
        }

        [Test]
        public void EatFood_WithSoundsEnabled_PublishesPlaySfxEvent()
        {
            _snakeModel.EatFood();

            var sfxEvent = _mockEventBus.GetStructEvent<PlaySfxEvent>();
            Assert.IsNotNull(sfxEvent);
            Assert.AreEqual(SoundClipName.SnakeEat, sfxEvent.Value.ClipName);
        }

        [Test]
        public void Move_OnFoodPosition_CallsEatFood()
        {
            var foodPosition = new Vector2Int(11, 10); // Next position when moving right
            _snakeModel.FoodPosition = foodPosition;
            
            _snakeModel.Move();

            var foodEatenEvent = _mockEventBus.GetStructEvent<FoodEatenEvent>();
            Assert.IsNotNull(foodEatenEvent);
        }

        #endregion

        #region Grid Wrapping Tests

        [Test]
        public void Move_BeyondRightBoundary_WrapsToLeft()
        {
            // Position snake at right edge
            var config = CreateCustomSnakeConfig(Direction.Right, new Vector2Int(19, 10));
            var disposables = new CompositeDisposable();
            var snakeModel = new SnakeModel(_mockEventBus, config, disposables, _gameConfig);
            
            snakeModel.Move();
            
            Assert.AreEqual(0, snakeModel.HeadPosition.Value.x);
            
            snakeModel.Dispose();
            disposables.Dispose();
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void Move_BeyondLeftBoundary_WrapsToRight()
        {
            var config = CreateCustomSnakeConfig(Direction.Left, new Vector2Int(0, 10));
            var disposables = new CompositeDisposable();
            var snakeModel = new SnakeModel(_mockEventBus, config, disposables, _gameConfig);
            
            snakeModel.Move();
            
            Assert.AreEqual(19, snakeModel.HeadPosition.Value.x);
            
            snakeModel.Dispose();
            disposables.Dispose();
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void Move_BeyondTopBoundary_WrapsToBottom()
        {
            var config = CreateCustomSnakeConfig(Direction.Up, new Vector2Int(10, 19));
            var disposables = new CompositeDisposable();
            var snakeModel = new SnakeModel(_mockEventBus, config, disposables, _gameConfig);
            
            snakeModel.Move();
            
            Assert.AreEqual(0, snakeModel.HeadPosition.Value.y);
            
            snakeModel.Dispose();
            disposables.Dispose();
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void Move_BeyondBottomBoundary_WrapsToTop()
        {
            var config = CreateCustomSnakeConfig(Direction.Down, new Vector2Int(10, 0));
            var disposables = new CompositeDisposable();
            var snakeModel = new SnakeModel(_mockEventBus, config, disposables, _gameConfig);
            
            snakeModel.Move();
            
            Assert.AreEqual(19, snakeModel.HeadPosition.Value.y);
            
            snakeModel.Dispose();
            disposables.Dispose();
            UnityEngine.Object.DestroyImmediate(config);
        }

        #endregion

        #region Self-Collision Tests

        [Test]
        public void Move_IntoSelfBody_CausesSnakeToDie()
        {
            // Grow snake first
            _snakeModel.EatFood();
            _snakeModel.EatFood();
            _snakeModel.EatFood();
            
            // Move to create body
            _snakeModel.Move();
            _snakeModel.Move();
            _snakeModel.Move();
            
            // Turn around to hit body
            _snakeModel.SetDirection(Direction.Up);
            _snakeModel.Move();
            _snakeModel.SetDirection(Direction.Left);
            _snakeModel.Move();
            _snakeModel.SetDirection(Direction.Down);
            _snakeModel.Move();
            
            Assert.AreEqual(SnakeState.Dead, _snakeModel.State.Value);
        }

        #endregion

        #region Body Position Tests

        [Test]
        public void BodyPositions_AfterEatingAndMoving_ContainsCorrectPositions()
        {
            _snakeModel.EatFood();
            var positionBeforeMove = _snakeModel.HeadPosition.Value;
            
            _snakeModel.Move();
            
            Assert.AreEqual(1, _snakeModel.BodyPositions.Value.Count);
            Assert.AreEqual(positionBeforeMove, _snakeModel.BodyPositions.Value[0].GridPosition);
        }

        [Test]
        public void GetAllOccupiedPositions_IncludesHeadAndBody()
        {
            _snakeModel.EatFood();
            _snakeModel.Move();
            
            var occupiedPositions = _snakeModel.GetAllOccupiedPositions();
            
            Assert.Contains(_snakeModel.HeadPosition.Value, occupiedPositions);
            foreach (var bodyPos in _snakeModel.BodyPositions.Value)
            {
                Assert.Contains(bodyPos.GridPosition, occupiedPositions);
            }
        }

        #endregion

        #region Utility Tests

        [Test]
        public void GetAngleFromDirection_ReturnsCorrectAngles()
        {
            Assert.AreEqual(0f, _snakeModel.GetAngleFromDirection(Direction.Right));
            Assert.AreEqual(90f, _snakeModel.GetAngleFromDirection(Direction.Up));
            Assert.AreEqual(180f, _snakeModel.GetAngleFromDirection(Direction.Left));
            Assert.AreEqual(270f, _snakeModel.GetAngleFromDirection(Direction.Down));
        }

        #endregion

        #region Reactive Property Tests

        [Test]
        public void State_CanBeSubscribed()
        {
            SnakeState capturedState = SnakeState.Dead;
            bool wasNotified = false;

            _snakeModel.State.Subscribe(state =>
            {
                capturedState = state;
                wasNotified = true;
            });

            Assert.IsTrue(wasNotified);
            Assert.AreEqual(SnakeState.Alive, capturedState);
        }

        [Test]
        public void HeadPosition_CanBeSubscribed()
        {
            Vector2Int capturedPosition = Vector2Int.zero;
            bool wasNotified = false;

            _snakeModel.HeadPosition.Subscribe(position =>
            {
                capturedPosition = position;
                wasNotified = true;
            });

            Assert.IsTrue(wasNotified);
            Assert.AreEqual(_snakeConfig.StartPosition, capturedPosition);
        }

        [Test]
        public void CurrentDirection_CanBeSubscribed()
        {
            Direction capturedDirection = Direction.Up;
            bool wasNotified = false;

            _snakeModel.CurrentDirection.Subscribe(dir =>
            {
                capturedDirection = dir;
                wasNotified = true;
            });

            Assert.IsTrue(wasNotified);
            Assert.AreEqual(_snakeConfig.StartDirection, capturedDirection);
        }

        [Test]
        public void BodyPositions_CanBeSubscribed()
        {
            IReadOnlyList<SnakeMovePosition> capturedPositions = null;
            bool wasNotified = false;

            _snakeModel.BodyPositions.Subscribe(positions =>
            {
                capturedPositions = positions;
                wasNotified = true;
            });

            Assert.IsTrue(wasNotified);
            Assert.IsNotNull(capturedPositions);
            Assert.AreEqual(0, capturedPositions.Count);
        }

        #endregion

        #region Configuration Tests

        [Test]
        public void Constructor_WithDifferentConfigurations_WorksCorrectly()
        {
            var testCases = new[]
            {
                (Direction.Up, new Vector2Int(5, 5)),
                (Direction.Down, new Vector2Int(15, 15)),
                (Direction.Left, new Vector2Int(0, 0)),
                (Direction.Right, new Vector2Int(19, 19))
            };

            foreach (var (direction, position) in testCases)
            {
                var config = CreateCustomSnakeConfig(direction, position);
                var disposables = new CompositeDisposable();
                var snakeModel = new SnakeModel(_mockEventBus, config, disposables, _gameConfig);

                Assert.AreEqual(direction, snakeModel.CurrentDirection.Value);
                Assert.AreEqual(position, snakeModel.HeadPosition.Value);

                snakeModel.Dispose();
                disposables.Dispose();
                UnityEngine.Object.DestroyImmediate(config);
            }
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => _snakeModel.Dispose());
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            Assert.DoesNotThrow(() =>
            {
                _snakeModel.Dispose();
                _snakeModel.Dispose();
                _snakeModel.Dispose();
            });
        }

        #endregion

        #region Helper Methods

        private SnakeConfig CreateCustomSnakeConfig(Direction direction, Vector2Int position)
        {
            var config = ScriptableObject.CreateInstance<SnakeConfig>();
            config.StartDirection = direction;
            config.StartPosition = position;
            config.BodyPartSortingOrder = 1;
            config.EnableSounds = true;
            return config;
        }

        #endregion
    }

    #region Mock Classes

    public class MockEventBus : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        public void Publish<T>(T eventData)
        {
            PublishedEvents.Add(eventData);
        }
        
        public T? GetStructEvent<T>() where T : struct
        {
            return PublishedEvents.OfType<T>().FirstOrDefault();
        }
        
        public IObservable<T> OnEvent<T>()
        {
            return Observable.Empty<T>();
        }

        public void Dispose()
        {
            PublishedEvents.Clear();
        }
    }

    #endregion
}