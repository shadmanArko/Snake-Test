using UnityEngine;

namespace SnakeSystem
{
    public interface ILevelGrid
    {
        Vector2Int ValidateGridPosition(Vector2Int position);
        bool TrySnakeEatFood(Vector2Int position);
    }
}