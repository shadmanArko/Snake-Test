using _Scripts.Enums;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Scripts.Services.InputSystem
{
    public class PCInput : IGameInput, ITickable
    {
        public ReactiveProperty<Direction?> DirectionInput { get; private set; }
    
        private Direction? _lastDirection;

        public PCInput()
        {
            DirectionInput = new ReactiveProperty<Direction?>(null);
        }

        public void Tick()
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
    }
}