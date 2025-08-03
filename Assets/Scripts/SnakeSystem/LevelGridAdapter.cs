using LevelSystem.Model;
using UnityEngine;

namespace SnakeSystem
{
    public class LevelGridAdapter : ILevelGrid
    {
        private readonly IGameplayModel _model;

        public LevelGridAdapter(IGameplayModel model)
        {
            _model = model;
        }

        public Vector2Int ValidateGridPosition(Vector2Int position)
        {
            return _model.ValidateGridPosition(position);
        }

        public bool TrySnakeEatFood(Vector2Int position)
        {
            return _model.TryEatFood(position);
        }
    }
}