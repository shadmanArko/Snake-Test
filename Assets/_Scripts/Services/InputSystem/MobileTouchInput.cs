using _Scripts.Enums;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Scripts.Services.InputSystem
{
    public class MobileTouchInput : IGameInput, ITickable
    {
        public ReactiveProperty<Direction?> DirectionInput { get; private set; }

        private Vector2 _touchStartPos;
        private bool _isTouching = false;
        private Direction? _lastDirection;

        // Minimum swipe distance to register as input
        private readonly float _minSwipeDistance = 50f;

        public MobileTouchInput()
        {
            DirectionInput = new ReactiveProperty<Direction?>(null);
        }

        public void Tick()
        {
            HandleTouchInput();
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _touchStartPos = touch.position;
                        _isTouching = true;
                        break;

                    case TouchPhase.Ended:
                        if (_isTouching)
                        {
                            Vector2 touchEndPos = touch.position;
                            ProcessSwipe(_touchStartPos, touchEndPos);
                            _isTouching = false;
                        }

                        break;

                    case TouchPhase.Canceled:
                        _isTouching = false;
                        break;
                }
            }

            // Handle mouse input for testing in editor
#if UNITY_EDITOR
            HandleMouseInput();
#endif
        }

#if UNITY_EDITOR
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _touchStartPos = Input.mousePosition;
                _isTouching = true;
            }
            else if (Input.GetMouseButtonUp(0) && _isTouching)
            {
                Vector2 mouseEndPos = Input.mousePosition;
                ProcessSwipe(_touchStartPos, mouseEndPos);
                _isTouching = false;
            }
        }
#endif

        private void ProcessSwipe(Vector2 startPos, Vector2 endPos)
        {
            Vector2 swipeVector = endPos - startPos;
            float swipeDistance = swipeVector.magnitude;

            // Check if swipe distance meets minimum threshold
            if (swipeDistance < _minSwipeDistance)
                return;

            // Determine swipe direction based on the larger axis
            Direction? newDirection = null;

            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                // Horizontal swipe
                newDirection = swipeVector.x > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                // Vertical swipe
                newDirection = swipeVector.y > 0 ? Direction.Up : Direction.Down;
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