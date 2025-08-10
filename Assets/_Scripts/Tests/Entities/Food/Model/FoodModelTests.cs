using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using UniRx;
using _Scripts.Entities.Food.Model;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using _Scripts.GlobalConfigs;

namespace _Scripts.Tests.Entities.Food.Model
{
    [TestFixture]
    public class FoodModelTests
    {
        private MockEventBus _mockEventBus;
        private GameConfig _gameConfig;
        private FoodModel _foodModel;
        private CompositeDisposable _disposables;
        private const string TestSpriteKey = "test_food_sprite";

        [SetUp]
        public void SetUp()
        {
            SetupGameConfig();
            SetupDependencies();
            CreateFoodModel();
        }

        [TearDown]
        public void TearDown()
        {
            CleanupFoodModel();
            CleanupDependencies();
            CleanupConfigs();
        }

        #region Setup/Teardown Helpers

        private void SetupGameConfig()
        {
            _gameConfig = ScriptableObject.CreateInstance<GameConfig>();
            _gameConfig.gridWidth = 10;
            _gameConfig.gridHeight = 10;
            _gameConfig.foodSpriteAddressableKey = TestSpriteKey;
            _gameConfig.maxFoodSpawnAttempts = 100;
        }

        private void SetupDependencies()
        {
            _disposables = new CompositeDisposable();
            _mockEventBus = new MockEventBus();
        }

        private void CreateFoodModel()
        {
            _foodModel = new FoodModel(_mockEventBus, _gameConfig, _disposables);
        }

        private void CleanupFoodModel()
        {
            _foodModel?.Dispose();
        }

        private void CleanupDependencies()
        {
            _disposables?.Dispose();
        }

        private void CleanupConfigs()
        {
            if (_gameConfig != null)
                UnityEngine.Object.DestroyImmediate(_gameConfig);
        }

        #endregion

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidDependencies_InitializesCorrectly()
        {
            Assert.IsNotNull(_foodModel);
            Assert.IsNotNull(_foodModel.FoodPosition);
        }

        [Test]
        public void Constructor_InitializesWithZeroPosition()
        {
            Assert.AreEqual(Vector2Int.zero, _foodModel.FoodPosition.Value);
        }

        [Test]
        public void Constructor_WithDifferentConfigurations_WorksCorrectly()
        {
            var testConfigs = new[]
            {
                (gridWidth: 5, gridHeight: 5, maxAttempts: 50),
                (gridWidth: 20, gridHeight: 15, maxAttempts: 200),
                (gridWidth: 1, gridHeight: 1, maxAttempts: 10)
            };

            foreach (var (gridWidth, gridHeight, maxAttempts) in testConfigs)
            {
                var config = CreateCustomGameConfig(gridWidth, gridHeight, maxAttempts);
                var disposables = new CompositeDisposable();
                var foodModel = new FoodModel(_mockEventBus, config, disposables);

                Assert.IsNotNull(foodModel);
                Assert.AreEqual(Vector2Int.zero, foodModel.FoodPosition.Value);

                foodModel.Dispose();
                disposables.Dispose();
                UnityEngine.Object.DestroyImmediate(config);
            }
        }

        #endregion

        #region SpawnFood Tests

        [Test]
        public void SpawnFood_WithEmptyOccupiedPositions_SetsValidPosition()
        {
            var occupiedPositions = new List<Vector2Int>();
            
            _foodModel.SpawnFood(occupiedPositions);

            var position = _foodModel.FoodPosition.Value;
            Assert.IsTrue(IsValidGridPosition(position));
        }

        [Test]
        public void SpawnFood_WithOccupiedPositions_AvoidsOccupiedPositions()
        {
            var occupiedPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 1),
                new Vector2Int(2, 2),
                new Vector2Int(3, 3)
            };

            _foodModel.SpawnFood(occupiedPositions);

            var finalPosition = _foodModel.FoodPosition.Value;
            Assert.IsFalse(occupiedPositions.Contains(finalPosition));
            Assert.IsTrue(IsValidGridPosition(finalPosition));
        }

        [Test]
        public void SpawnFood_MultipleCalls_GeneratesDifferentPositions()
        {
            var positions = new List<Vector2Int>();
            var occupiedPositions = new List<Vector2Int>();

            // Generate multiple food positions
            for (int i = 0; i < 10; i++)
            {
                _foodModel.SpawnFood(occupiedPositions);
                var position = _foodModel.FoodPosition.Value;
                positions.Add(position);
                occupiedPositions.Add(position); // Add to occupied to force different positions
            }

            // Verify we got different positions (allowing for some randomness)
            var uniquePositions = positions.Distinct().Count();
            Assert.Greater(uniquePositions, 1, "Should generate different positions across multiple spawns");
        }

        [Test]
        public void SpawnFood_WithNearFullGrid_FindsAvailablePosition()
        {
            var config = CreateCustomGameConfig(3, 3, 100);
            var disposables = new CompositeDisposable();
            var foodModel = new FoodModel(_mockEventBus, config, disposables);

            // Occupy 8 out of 9 positions
            var occupiedPositions = new List<Vector2Int>();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x != 2 || y != 2) // Leave (2,2) available
                    {
                        occupiedPositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            foodModel.SpawnFood(occupiedPositions);

            Assert.AreEqual(new Vector2Int(2, 2), foodModel.FoodPosition.Value);

            foodModel.Dispose();
            disposables.Dispose();
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void SpawnFood_WithCompletelyFullGrid_StillSetsPosition()
        {
            var config = CreateCustomGameConfig(2, 2, 50);
            var disposables = new CompositeDisposable();
            var foodModel = new FoodModel(_mockEventBus, config, disposables);

            // Occupy all positions
            var occupiedPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1)
            };

            // Should not hang indefinitely due to maxFoodSpawnAttempts
            Assert.DoesNotThrow(() => foodModel.SpawnFood(occupiedPositions));

            // Position should still be within grid bounds
            var position = foodModel.FoodPosition.Value;
            Assert.IsTrue(position.x >= 0 && position.x < 2);
            Assert.IsTrue(position.y >= 0 && position.y < 2);

            foodModel.Dispose();
            disposables.Dispose();
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void SpawnFood_PublishesFoodSpawnedEvent()
        {
            var occupiedPositions = new List<Vector2Int>();

            _foodModel.SpawnFood(occupiedPositions);

            var spawnedEvent = _mockEventBus.GetStructEvent<FoodSpawnedEvent>();
            Assert.IsNotNull(spawnedEvent);
            Assert.AreEqual(_foodModel.FoodPosition.Value, spawnedEvent.Value.Position);
        }

        [Test]
        public void SpawnFood_UpdatesFoodPositionReactiveProperty()
        {
            var positionUpdates = new List<Vector2Int>();
            _foodModel.FoodPosition.Subscribe(pos => positionUpdates.Add(pos));

            var occupiedPositions = new List<Vector2Int>();
            _foodModel.SpawnFood(occupiedPositions);

            Assert.AreEqual(2, positionUpdates.Count); // Initial + spawned
            Assert.AreEqual(Vector2Int.zero, positionUpdates[0]); // Initial
            Assert.AreNotEqual(Vector2Int.zero, positionUpdates[1]); // Spawned
        }

        [Test]
        public void SpawnFood_WithValidOccupiedPositions_RespectsBoundaries()
        {
            var occupiedPositions = new List<Vector2Int>
            {
                new Vector2Int(-1, -1), // Invalid positions should not affect spawning
                new Vector2Int(20, 20),
                new Vector2Int(1, 1)    // Valid occupied position
            };

            _foodModel.SpawnFood(occupiedPositions);

            var position = _foodModel.FoodPosition.Value;
            Assert.IsTrue(IsValidGridPosition(position));
            Assert.AreNotEqual(new Vector2Int(1, 1), position);
        }

        #endregion

        #region Reactive Property Tests

        [Test]
        public void FoodPosition_IsReadOnlyReactiveProperty()
        {
            Assert.IsNotNull(_foodModel.FoodPosition);
            Assert.IsInstanceOf<IReadOnlyReactiveProperty<Vector2Int>>(_foodModel.FoodPosition);
        }

        [Test]
        public void FoodPosition_InitialValueIsZero()
        {
            Assert.AreEqual(Vector2Int.zero, _foodModel.FoodPosition.Value);
        }

        [Test]
        public void FoodPosition_CanBeSubscribed()
        {
            Vector2Int capturedPosition = Vector2Int.one; // Different from initial
            bool wasNotified = false;

            _foodModel.FoodPosition.Subscribe(pos =>
            {
                capturedPosition = pos;
                wasNotified = true;
            });

            Assert.IsTrue(wasNotified);
            Assert.AreEqual(Vector2Int.zero, capturedPosition);
        }

        [Test]
        public void FoodPosition_NotifiesOnPositionChange()
        {
            var notifications = new List<Vector2Int>();
            _foodModel.FoodPosition.Subscribe(pos => notifications.Add(pos));

            _foodModel.SpawnFood(new List<Vector2Int>());

            Assert.AreEqual(2, notifications.Count);
            Assert.AreEqual(Vector2Int.zero, notifications[0]);
            Assert.AreNotEqual(Vector2Int.zero, notifications[1]);
        }

        #endregion

        #region Event Publishing Tests

        [Test]
        public void SpawnFood_PublishesCorrectEventType()
        {
            _foodModel.SpawnFood(new List<Vector2Int>());

            Assert.AreEqual(1, _mockEventBus.PublishedEvents.Count);
            Assert.IsInstanceOf<FoodSpawnedEvent>(_mockEventBus.PublishedEvents[0]);
        }

        [Test]
        public void SpawnFood_EventContainsCorrectPosition()
        {
            _foodModel.SpawnFood(new List<Vector2Int>());

            var spawnedEvent = _mockEventBus.GetStructEvent<FoodSpawnedEvent>();
            var actualPosition = _foodModel.FoodPosition.Value;

            if (spawnedEvent != null) Assert.AreEqual(actualPosition, spawnedEvent.Value.Position);
        }
        

        #endregion

        #region Grid Boundary Tests

        [Test]
        public void SpawnFood_AlwaysGeneratesPositionWithinBounds()
        {
            var testConfigs = new[]
            {
                (width: 1, height: 1),
                (width: 5, height: 3),
                (width: 10, height: 10),
                (width: 20, height: 15)
            };

            foreach (var (width, height) in testConfigs)
            {
                var config = CreateCustomGameConfig(width, height, 100);
                var disposables = new CompositeDisposable();
                var foodModel = new FoodModel(_mockEventBus, config, disposables);

                // Test multiple spawns to ensure consistency
                for (int i = 0; i < 10; i++)
                {
                    foodModel.SpawnFood(new List<Vector2Int>());
                    var position = foodModel.FoodPosition.Value;
                    
                    Assert.IsTrue(position.x >= 0 && position.x < width, 
                        $"X position {position.x} should be within [0, {width})");
                    Assert.IsTrue(position.y >= 0 && position.y < height, 
                        $"Y position {position.y} should be within [0, {height})");
                }

                foodModel.Dispose();
                disposables.Dispose();
                UnityEngine.Object.DestroyImmediate(config);
            }
        }

        #endregion

        #region Sprite Loading Tests

        [Test]
        public void LoadFoodSprite_DoesNotThrowException()
        {
            Assert.DoesNotThrow(async () => await _foodModel.LoadFoodSprite());
        }

        [Test]
        public void FoodSprite_InitiallyNull()
        {
            Assert.IsNull(_foodModel.FoodSprite);
        }

        #endregion

        #region Performance Tests

        [Test]
        public void SpawnFood_WithManyOccupiedPositions_CompletesInReasonableTime()
        {
            var occupiedPositions = new List<Vector2Int>();
            
            // Add many occupied positions
            for (int x = 0; x < _gameConfig.gridWidth; x++)
            {
                for (int y = 0; y < _gameConfig.gridHeight; y++)
                {
                    if (occupiedPositions.Count < (_gameConfig.gridWidth * _gameConfig.gridHeight) - 5)
                    {
                        occupiedPositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            var startTime = DateTime.Now;
            _foodModel.SpawnFood(occupiedPositions);
            var endTime = DateTime.Now;

            // Should complete within reasonable time (less than 1 second for test)
            Assert.IsTrue((endTime - startTime).TotalSeconds < 1.0);
        }

        #endregion

        #region Edge Case Tests
        

        [Test]
        public void SpawnFood_WithEmptyList_GeneratesValidPosition()
        {
            var emptyList = new List<Vector2Int>();
            
            _foodModel.SpawnFood(emptyList);
            
            Assert.IsTrue(IsValidGridPosition(_foodModel.FoodPosition.Value));
        }

        [Test]
        public void SpawnFood_ConsecutiveCalls_MaintainsEventConsistency()
        {
            var positions = new List<Vector2Int>();
            if (positions == null) throw new ArgumentNullException(nameof(positions));

            for (int i = 0; i < 5; i++)
            {
                _mockEventBus.PublishedEvents.Clear(); // Reset events
                _foodModel.SpawnFood(new List<Vector2Int>());
                
                Assert.AreEqual(1, _mockEventBus.PublishedEvents.Count);
                var spawnedEvent = _mockEventBus.GetStructEvent<FoodSpawnedEvent>();
                if (spawnedEvent != null) Assert.AreEqual(_foodModel.FoodPosition.Value, spawnedEvent.Value.Position);

                positions.Add(_foodModel.FoodPosition.Value);
            }
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => _foodModel.Dispose());
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            Assert.DoesNotThrow(() =>
            {
                _foodModel.Dispose();
                _foodModel.Dispose();
                _foodModel.Dispose();
            });
        }

        [Test]
        public void Dispose_AfterSpawning_DoesNotThrow()
        {
            _foodModel.SpawnFood(new List<Vector2Int>());
            
            Assert.DoesNotThrow(() => _foodModel.Dispose());
        }

        #endregion

        #region Configuration Tests

        [Test]
        public void Constructor_WithDifferentMaxAttempts_WorksCorrectly()
        {
            var testMaxAttempts = new[] { 1, 10, 50, 100, 500 };

            foreach (var maxAttempts in testMaxAttempts)
            {
                var config = CreateCustomGameConfig(5, 5, maxAttempts);
                var disposables = new CompositeDisposable();
                var foodModel = new FoodModel(_mockEventBus, config, disposables);

                // Should work regardless of max attempts setting
                Assert.DoesNotThrow(() => foodModel.SpawnFood(new List<Vector2Int>()));
                Assert.IsTrue(IsValidGridPosition(foodModel.FoodPosition.Value, 5, 5));

                foodModel.Dispose();
                disposables.Dispose();
                UnityEngine.Object.DestroyImmediate(config);
            }
        }

        #endregion

        #region Helper Methods

        private bool IsValidGridPosition(Vector2Int position)
        {
            return IsValidGridPosition(position, _gameConfig.gridWidth, _gameConfig.gridHeight);
        }

        private bool IsValidGridPosition(Vector2Int position, int gridWidth, int gridHeight)
        {
            return position.x >= 0 && position.x < gridWidth && 
                   position.y >= 0 && position.y < gridHeight;
        }

        private GameConfig CreateCustomGameConfig(int gridWidth, int gridHeight, int maxAttempts)
        {
            var config = ScriptableObject.CreateInstance<GameConfig>();
            config.gridWidth = gridWidth;
            config.gridHeight = gridHeight;
            config.maxFoodSpawnAttempts = maxAttempts;
            config.foodSpriteAddressableKey = TestSpriteKey;
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

        public T GetEvent<T>() where T : class
        {
            return PublishedEvents.OfType<T>().FirstOrDefault();
        }

        public List<T> GetEvents<T>() where T : class
        {
            return PublishedEvents.OfType<T>().ToList();
        }
        
        public T? GetStructEvent<T>() where T : struct
        {
            return PublishedEvents.OfType<T>().FirstOrDefault();
        }

        public IObservable<T> GetEventStream<T>() where T : struct
        {
            return Observable.Empty<T>();
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