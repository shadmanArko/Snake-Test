using _Scripts.Enums;
using UnityEngine;

namespace _Scripts.Entities.Snake.ValueObjects
{
    public struct SnakeMovePosition
    {
        public Vector2Int GridPosition { get; }
        public Direction CurrentDirection { get; }
        public Direction PreviousDirection { get; }

        public SnakeMovePosition(Vector2Int gridPosition, Direction currentDirection, Direction previousDirection)
        {
            GridPosition = gridPosition;
            CurrentDirection = currentDirection;
            PreviousDirection = previousDirection;
        }
    }
}