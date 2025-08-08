using _Scripts.Enums;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Scripts.Services.InputSystem
{
    public class PCInput : IGameInput, ITickable, IInitializable
    {
        public ReactiveProperty<Direction?> DirectionInput { get; private set; }
    
        private Direction? _lastDirection;
        private bool _isPaused = false;

        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables;
        
        public PCInput(IEventBus eventBus, CompositeDisposable disposables)
        {
            _eventBus = eventBus;
            _disposables = disposables;
            DirectionInput = new ReactiveProperty<Direction?>(null);
        }
        
        public void Initialize()
        {
            _eventBus.OnEvent<PauseGameEvent>().Subscribe(_=> _isPaused = true).AddTo(_disposables);
            _eventBus.OnEvent<ResumeGameEvent>().Subscribe(_=> _isPaused = false).AddTo(_disposables);
        }

        public void Tick()
        {
            HandleDirectionalInput();
            HandlePauseInput();
        }

        private void HandleDirectionalInput()
        {
            Direction? newDirection = null;

            // Check for arrow keys
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                newDirection = Direction.Up;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                newDirection = Direction.Down;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                newDirection = Direction.Left;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                newDirection = Direction.Right;
            }

            // Only update if we have a new direction and it's different from the last one
            if (newDirection.HasValue && newDirection != _lastDirection)
            {
                _lastDirection = newDirection;
                DirectionInput.Value = newDirection;
            }
        }
        
        private void HandlePauseInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                {
                    _eventBus.Publish(new ResumeGameEvent());
                }
                else
                {
                    _eventBus.Publish(new PauseGameEvent());
                }
            }
        }
    }
}