using System.Collections.Generic;
using UnityEngine;

namespace LevelSystem.Events
{
    public struct FoodEatenEvent
    {
        public Vector2Int Position;
        public List<Vector2Int> OccupiedPositions;
    }
}