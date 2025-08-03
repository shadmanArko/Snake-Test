using UnityEngine;

namespace SnakeSystem
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