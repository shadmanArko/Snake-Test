using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UniRx;
using _Scripts.Entities.Food.Config;
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
        private FoodConfig _testConfig;
        private GameConfig _testGameConfig;
        private FoodModel _foodModel;
        private const string TestSpriteKey = "test_food_sprite";
        private CompositeDisposable _disposables;

        [SetUp]
        public void SetUp()
        {
            // Create test config instances
            _testGameConfig = ScriptableObject.CreateInstance<GameConfig>();
            _testGameConfig.gridWidth = 10;
            _testGameConfig.gridHeight = 10;

            _testConfig = ScriptableObject.CreateInstance<FoodConfig>();
            _testConfig.GameConfig = _testGameConfig;
            _testConfig.GameConfig.foodSpriteAddressableKey = TestSpriteKey;

            // Setup mock event bus
            _mockEventBus = new MockEventBus();

            // Create the system under test
            _foodModel = new FoodModel(_mockEventBus, _testConfig, _disposables);
        }

        [TearDown]
        public void TearDown()
        {
            _foodModel?.Dispose();
            
            if (_testConfig != null)
                UnityEngine.Object.DestroyImmediate(_testConfig);
            if (_testGameConfig != null)
                UnityEngine.Object.DestroyImmediate(_testGameConfig);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidDependencies_InitializesCorrectly()
        {
            // Assert
            Assert.IsNotNull(_foodModel);
            Assert.IsNotNull(_foodModel.FoodPosition);
            Assert.AreEqual(Vector2Int.zero, _foodModel.FoodPosition.Value);
        }

        // [Test]
        // public void Constructor_WithNullEventBus_ThrowsNullReferenceException()
        // {
        //     // Act & Assert
        //     Assert.Throws<NullReferenceException>(() => new FoodModel(null, _testConfig));
        // }
        //
        // [Test]
        // public void Constructor_WithNullConfig_ThrowsNullReferenceException()
        // {
        //     // Act & Assert
        //     Assert.Throws<NullReferenceException>(() => new FoodModel(_mockEventBus, null));
        // }

        #endregion

        #region SpawnFood Tests

        [Test]
        public void SpawnFood_WithEmptyOccupiedPositions_SetsValidPosition()
        {
            // Arrange
            var occupiedPositions = new List<Vector2Int>();
            Vector2Int capturedPosition = Vector2Int.zero;
            
            _foodModel.FoodPosition.Subscribe(pos => capturedPosition = pos);

            // Act
            _foodModel.SpawnFood(occupiedPositions);

            // Assert
            Assert.IsTrue(capturedPosition.x >= 0 && capturedPosition.x < _testConfig.GameConfig.gridWidth);
            Assert.IsTrue(capturedPosition.y >= 0 && capturedPosition.y < _testConfig.GameConfig.gridHeight);
        }

        [Test]
        public void SpawnFood_WithOccupiedPositions_AvoidsOccupiedPositions()
        {
            // Arrange
            var occupiedPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 1),
                new Vector2Int(2, 2)
            };
            Vector2Int finalPosition = Vector2Int.zero;
            
            _foodModel.FoodPosition.Subscribe(pos => finalPosition = pos);

            // Act
            _foodModel.SpawnFood(occupiedPositions);

            // Assert
            Assert.IsFalse(occupiedPositions.Contains(finalPosition));
            Assert.IsTrue(finalPosition.x >= 0 && finalPosition.x < _testConfig.GameConfig.gridWidth);
            Assert.IsTrue(finalPosition.y >= 0 && finalPosition.y < _testConfig.GameConfig.gridHeight);
        }

        [Test]
        public void SpawnFood_PublishesFoodSpawnedEvent()
        {
            // Arrange
            var occupiedPositions = new List<Vector2Int>();

            // Act
            _foodModel.SpawnFood(occupiedPositions);

            // Assert
            Assert.AreEqual(1, _mockEventBus.PublishedEvents.Count);
            Assert.IsInstanceOf<FoodSpawnedEvent>(_mockEventBus.PublishedEvents[0]);
            
            var publishedEvent = (FoodSpawnedEvent)_mockEventBus.PublishedEvents[0];
            Assert.AreEqual(_foodModel.FoodPosition.Value, publishedEvent.Position);
        }

        [Test]
        public void SpawnFood_UpdatesFoodPositionReactiveProperty()
        {
            // Arrange
            var occupiedPositions = new List<Vector2Int>();
            var positionUpdates = new List<Vector2Int>();
            
            _foodModel.FoodPosition.Subscribe(pos => positionUpdates.Add(pos));

            // Act
            _foodModel.SpawnFood(occupiedPositions);

            // Assert
            Assert.AreEqual(2, positionUpdates.Count); // Initial value + new value
            Assert.AreEqual(Vector2Int.zero, positionUpdates[0]); // Initial
            Assert.AreNotEqual(Vector2Int.zero, positionUpdates[1]); // Updated
        }

        [Test]
        public void SpawnFood_WithSmallGridAndManyOccupiedPositions_DoesNotHangInfinitely()
        {
            // Arrange - Create a model with small grid
            var smallGridConfig = ScriptableObject.CreateInstance<GameConfig>();
            smallGridConfig.gridWidth = 3;
            smallGridConfig.gridHeight = 3;

            var smallConfig = ScriptableObject.CreateInstance<FoodConfig>();
            smallConfig.GameConfig = smallGridConfig;
            smallConfig.GameConfig.foodSpriteAddressableKey = TestSpriteKey;

            var smallFoodModel = new FoodModel(_mockEventBus, smallConfig, _disposables);
            
            // Occupy most positions
            var occupiedPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, 2),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(1, 2),
                new Vector2Int(2, 0),
                new Vector2Int(2, 1)
                // Leave (2,2) free
            };

            // Act & Assert - Should not hang indefinitely
            Assert.DoesNotThrow(() => smallFoodModel.SpawnFood(occupiedPositions));
            
            // Should place food at the only available position
            Assert.AreEqual(new Vector2Int(2, 2), smallFoodModel.FoodPosition.Value);
            
            // Cleanup
            smallFoodModel.Dispose();
            UnityEngine.Object.DestroyImmediate(smallConfig);
            UnityEngine.Object.DestroyImmediate(smallGridConfig);
        }

        #endregion

        #region GetVtoAsync Tests

        [Test]
        public void GetVtoAsync_DoesNotThrowException()
        {
            // Note: Since AddressableHelper is static and loads actual assets,
            // this test just ensures the method doesn't crash
            // In production, you might want to refactor AddressableHelper to be injectable
            
            // Act & Assert
            Assert.DoesNotThrow(async () => await _foodModel.LoadFoodSprite());
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_DoesNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _foodModel.Dispose());
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _foodModel.Dispose();
                _foodModel.Dispose();
                _foodModel.Dispose();
            });
        }

        #endregion

        #region Property Tests

        [Test]
        public void FoodPosition_IsReadOnlyReactiveProperty()
        {
            // Assert
            Assert.IsNotNull(_foodModel.FoodPosition);
            Assert.IsInstanceOf<IReadOnlyReactiveProperty<Vector2Int>>(_foodModel.FoodPosition);
        }

        [Test]
        public void FoodPosition_InitialValueIsZero()
        {
            // Assert
            Assert.AreEqual(Vector2Int.zero, _foodModel.FoodPosition.Value);
        }

        #endregion
    }

    #region Mock Classes

    // Simple mock implementation of IEventBus for testing
    public class MockEventBus : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        public void Publish<T>(T eventData)  // Removed: where T : struct
        {
            PublishedEvents.Add(eventData);
        }


        public IObservable<T> GetEventStream<T>() where T : struct
        {
            return Observable.Empty<T>();
        }

        public IObservable<T> OnEvent<T>()   // Removed: where T : struct
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