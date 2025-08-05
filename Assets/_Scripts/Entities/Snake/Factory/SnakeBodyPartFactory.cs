using _Scripts.Entities.Snake.Config;
using _Scripts.Entities.Snake.View;
using UnityEngine;

namespace _Scripts.Entities.Snake.Factory
{
    public class SnakeBodyPartFactory : ISnakeBodyPartFactory
    {
        private readonly SnakeConfig _config;
    
        public SnakeBodyPartFactory(SnakeConfig config)
        {
            _config = config;
        }
    
        public SnakeBodyPartView CreateBodyPart()
        {
            var bodyPartGO = new GameObject("SnakeBodyPart");
            var spriteRenderer = bodyPartGO.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _config.snakeBodySprite;
            spriteRenderer.sortingOrder = _config.bodyPartSortingOrder;
            return bodyPartGO.AddComponent<SnakeBodyPartView>();
        }
    }
}