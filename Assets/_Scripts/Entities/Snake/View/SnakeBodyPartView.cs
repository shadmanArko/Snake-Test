using _Scripts.Entities.Snake.ValueObjects;
using _Scripts.Enums;
using UnityEngine;

namespace _Scripts.Entities.Snake.View
{
    public class SnakeBodyPartView : MonoBehaviour
    {
        private const float CornerOffsetDistance = 0.2f;
        
        private Transform _cachedTransform;

        private void Awake()
        {
            _cachedTransform = transform;
        }

        public void UpdatePosition(SnakeMovePosition movePosition)
        {
            SetPosition(movePosition.GridPosition);
            SetRotation(movePosition);
            ApplyCornerOffset(movePosition);
        }

        private void SetPosition(Vector2Int gridPosition)
        {
            _cachedTransform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
        }

        private void SetRotation(SnakeMovePosition movePosition)
        {
            var angle = CalculateBodyPartAngle(movePosition);
            _cachedTransform.eulerAngles = new Vector3(0, 0, angle);
        }

        private float CalculateBodyPartAngle(SnakeMovePosition movePosition)
        {
            var current = movePosition.CurrentDirection;
            var previous = movePosition.PreviousDirection;

            return current != previous 
                ? GetCornerAngle(current, previous) 
                : GetStraightAngle(current);
        }

        private float GetStraightAngle(Direction direction)
        {
            return direction switch
            {
                Direction.Up => 0f,
                Direction.Down => 180f,
                Direction.Left => 90f,
                Direction.Right => -90f,
                _ => 0f
            };
        }

        private float GetCornerAngle(Direction current, Direction previous)
        {
            return (current, previous) switch
            {
                (Direction.Up, Direction.Left) => 45f,
                (Direction.Up, Direction.Right) => -45f,
                (Direction.Down, Direction.Left) => 135f,
                (Direction.Down, Direction.Right) => 225f,
                (Direction.Left, Direction.Down) => 135f,
                (Direction.Left, Direction.Up) => 45f,
                (Direction.Right, Direction.Down) => 225f,
                (Direction.Right, Direction.Up) => -45f,
                _ => 0f
            };
        }

        private void ApplyCornerOffset(SnakeMovePosition movePosition)
        {
            var current = movePosition.CurrentDirection;
            var previous = movePosition.PreviousDirection;

            if (current == previous) return;

            var offset = GetCornerOffset(current, previous);
            _cachedTransform.position += offset;
        }

        private Vector3 GetCornerOffset(Direction current, Direction previous)
        {
            return (current, previous) switch
            {
                (Direction.Up, Direction.Left) => new Vector3(CornerOffsetDistance, CornerOffsetDistance, 0),
                (Direction.Up, Direction.Right) => new Vector3(-CornerOffsetDistance, CornerOffsetDistance, 0),
                (Direction.Down, Direction.Left) => new Vector3(CornerOffsetDistance, -CornerOffsetDistance, 0),
                (Direction.Down, Direction.Right) => new Vector3(-CornerOffsetDistance, -CornerOffsetDistance, 0),
                (Direction.Left, Direction.Down) => new Vector3(-CornerOffsetDistance, CornerOffsetDistance, 0),
                (Direction.Left, Direction.Up) => new Vector3(-CornerOffsetDistance, -CornerOffsetDistance, 0),
                (Direction.Right, Direction.Down) => new Vector3(CornerOffsetDistance, CornerOffsetDistance, 0),
                (Direction.Right, Direction.Up) => new Vector3(CornerOffsetDistance, -CornerOffsetDistance, 0),
                _ => Vector3.zero
            };
        }
    }
}