using _Scripts.Enums;
using UnityEngine;

namespace _Scripts.Events
{
    public struct SnakeMoveEvent
    {
        public Vector2Int Position;
        public Direction Direction;
    }
}