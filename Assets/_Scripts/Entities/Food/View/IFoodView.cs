using UnityEngine;

namespace _Scripts.Entities.Food.View
{
    public interface IFoodView
    {
        void SetFoodPosition(Vector2Int position);
        void ApplyVto(Sprite sprite);
    }
}