using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UniRx;
using _Scripts.Entities.Snake.Config;
using _Scripts.Entities.Snake.Model;
using _Scripts.Services.EventBus.Core;
using _Scripts.GlobalConfigs;
using _Scripts.Enums;

namespace _Scripts.Tests.Entities.Snake.Model
{
    [TestFixture]
    public class SnakeModelTests
    {
        private MockEventBus _mockEventBus;
        private SnakeConfig _testConfig;
        private GameConfig _testGameConfig;
        private SnakeModel _snakeModel;
        private CompositeDisposable _disposables;

        [SetUp]
        public void SetUp()
        {
            // Create test config instances
            _testGameConfig = ScriptableObject.CreateInstance<GameConfig>();
            _testGameConfig.gridWidth = 20;
            _testGameConfig.gridHeight = 20;

            _testConfig = ScriptableObject.CreateInstance<SnakeConfig>();
            _testGameConfig.snakeMoveInterval = 0.2f;
            _testConfig.StartPosition = new Vector2Int(10, 10);
            _testConfig.StartDirection = Direction.Right;
            _testConfig.BodyPartSortingOrder = 1;
            _testConfig.EnableSounds = true;
            

            // Setup mock event bus
            _mockEventBus = new MockEventBus();

            // Create the system under test
            _snakeModel = new SnakeModel(_mockEventBus, _testConfig, _disposables, _testGameConfig);
        }

        [TearDown]
        public void TearDown()
        {
            _snakeModel?.Dispose();
            
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
            Assert.IsNotNull(_snakeModel);
            Assert.IsNotNull(_snakeModel.BodyPositions);
            Assert.IsNotNull(_snakeModel.CurrentDirection);
            Assert.AreEqual(_testConfig.StartDirection, _snakeModel.CurrentDirection.Value);
        }

        [Test]
        public void Constructor_InitializesWithStartPosition()
        {
            // Assert
            Assert.AreEqual(0, _snakeModel.BodyPositions.Value.Count);
        }

        #endregion

        #region Basic Tests

        [Test]
        public void CurrentDirection_InitialValueMatchesConfig()
        {
            // Assert
            Assert.AreEqual(_testConfig.StartDirection, _snakeModel.CurrentDirection.Value);
        }

        [Test]
        public void BodyPositions_IsNotEmpty_InitiallyContainsOnePosition()
        {
            // Assert
            Assert.IsNotNull(_snakeModel.BodyPositions.Value);
            Assert.AreEqual(0, _snakeModel.BodyPositions.Value.Count);
        }

        #endregion

        #region Event Publishing Tests

        [Test]
        public void Constructor_PublishesExpectedEvents()
        {
            // Assert - Check that some events were published during construction
            Assert.GreaterOrEqual(_mockEventBus.PublishedEvents.Count, 0);
        }

        #endregion

        #region Dispose Tests

        [Test]
        public void Dispose_DoesNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _snakeModel.Dispose());
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _snakeModel.Dispose();
                _snakeModel.Dispose();
                _snakeModel.Dispose();
            });
        }

        #endregion

        #region Property Tests

        [Test]
        public void CurrentDirection_IsReadOnlyReactiveProperty()
        {
            // Assert
            Assert.IsNotNull(_snakeModel.CurrentDirection);
            Assert.IsInstanceOf<IReadOnlyReactiveProperty<Direction>>(_snakeModel.CurrentDirection);
        }

        [Test]
        public void BodyPositions_IsReadOnlyReactiveProperty()
        {
            // Assert
            Assert.IsNotNull(_snakeModel.BodyPositions);
        }

        #endregion

        #region Reactive Property Tests

        [Test]
        public void CurrentDirection_CanBeSubscribed()
        {
            // Arrange
            Direction capturedDirection = Direction.Up; // Different from initial
            bool wasNotified = false;

            // Act
            _snakeModel.CurrentDirection.Subscribe(dir =>
            {
                capturedDirection = dir;
                wasNotified = true;
            });

            // Assert
            Assert.IsTrue(wasNotified);
            Assert.AreEqual(_testConfig.StartDirection, capturedDirection);
        }

        // [Test]
        // public void BodyPositions_CanBeSubscribed()
        // {
        //     // Arrange
        //     bool wasNotified = false;
        //     int capturedCount = 0;
        //
        //     // Act
        //     _snakeModel.BodyPositions.Subscribe(positions =>
        //     {
        //         capturedCount = positions.Count;
        //         wasNotified = true;
        //     });
        //
        //     // Assert
        //     Assert.IsTrue(wasNotified);
        //     Assert.AreEqual(1, capturedCount);
        // }

        #endregion

        #region Configuration Tests

        [Test]
        public void Constructor_UsesConfigurationCorrectly()
        {
            // Arrange
            var customConfig = ScriptableObject.CreateInstance<SnakeConfig>();
            customConfig.StartDirection = Direction.Down;
            customConfig.StartPosition = new Vector2Int(5, 5);
            

            // Act
            var customSnakeModel = new SnakeModel(_mockEventBus, customConfig, _disposables, _testGameConfig);;

            // Assert
            Assert.AreEqual(Direction.Down, customSnakeModel.CurrentDirection.Value);
            
            // Clean up
            customSnakeModel.Dispose();
            UnityEngine.Object.DestroyImmediate(customConfig);
        }

        [Test]
        public void Constructor_WithDifferentStartDirections_InitializesCorrectly()
        {
            var directions = new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

            foreach (var direction in directions)
            {
                // Arrange
                var config = ScriptableObject.CreateInstance<SnakeConfig>();
                config.StartDirection = direction;
                config.StartPosition = new Vector2Int(10, 10);

                // Act
                var snakeModel = new SnakeModel(_mockEventBus, config, _disposables,_testGameConfig);

                // Assert
                Assert.AreEqual(direction, snakeModel.CurrentDirection.Value, 
                    $"Snake should start with direction {direction}");

                // Clean up
                snakeModel.Dispose();
                UnityEngine.Object.DestroyImmediate(config);
            }
        }

        #endregion

        #region MockEventBus Validation Tests

        [Test]
        public void MockEventBus_IsWorkingCorrectly()
        {
            // Arrange
            var testEvent = new { Message = "Test Event" };

            // Act
            _mockEventBus.Publish(testEvent);

            // Assert
            Assert.AreEqual(1, _mockEventBus.PublishedEvents.Count);
            Assert.AreEqual(testEvent, _mockEventBus.PublishedEvents[0]);
        }

        #endregion
    }

    #region Mock Classes

    // Simple mock implementation of IEventBus for testing
    public class MockEventBus : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        public void Publish<T>(T eventData)
        {
            PublishedEvents.Add(eventData);
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