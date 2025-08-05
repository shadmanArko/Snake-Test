using _Scripts.Entities.Snake.Enums;
using _Scripts.Entities.Snake.ValueObjects;
using UnityEngine;

namespace _Scripts.Entities.Snake.View
{
    public class SnakeBodyPartView : MonoBehaviour
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void UpdatePosition(SnakeMovePosition movePosition)
        {
            _transform.position = new Vector3(movePosition.GridPosition.x, movePosition.GridPosition.y, 0);

            var angle = CalculateBodyPartAngle(movePosition);
            _transform.eulerAngles = new Vector3(0, 0, angle);

            // Apply corner offset for smooth curves
            ApplyCornerOffset(movePosition);
        }

        private float CalculateBodyPartAngle(SnakeMovePosition movePosition)
        {
            var current = movePosition.CurrentDirection;
            var previous = movePosition.PreviousDirection;

            // Handle corner pieces
            if (current != previous)
            {
                return GetCornerAngle(current, previous);
            }

            // Straight pieces
            return current switch
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

            if (current == previous) return; // No offset for straight pieces

            var offset = (current, previous) switch
            {
                (Direction.Up, Direction.Left) => new Vector3(0.2f, 0.2f, 0),
                (Direction.Up, Direction.Right) => new Vector3(-0.2f, 0.2f, 0),
                (Direction.Down, Direction.Left) => new Vector3(0.2f, -0.2f, 0),
                (Direction.Down, Direction.Right) => new Vector3(-0.2f, -0.2f, 0),
                (Direction.Left, Direction.Down) => new Vector3(-0.2f, 0.2f, 0),
                (Direction.Left, Direction.Up) => new Vector3(-0.2f, -0.2f, 0),
                (Direction.Right, Direction.Down) => new Vector3(0.2f, 0.2f, 0),
                (Direction.Right, Direction.Up) => new Vector3(0.2f, -0.2f, 0),
                _ => Vector3.zero
            };

            _transform.position += offset;
        }
    }
}