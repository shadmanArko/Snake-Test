using _Scripts.Entities.Snake.Config;
using _Scripts.Entities.Snake.View;
using UnityEngine;

namespace _Scripts.Entities.Snake.Factory
{
    public class SnakeBodyPartFactory : ISnakeBodyPartFactory
    {
        private readonly ISnakeConfig _config;
    
        public SnakeBodyPartFactory(ISnakeConfig config)
        {
            _config = config;
        }
    
        public SnakeBodyPartView CreateBodyPart()
        {
            var bodyPartGO = new GameObject("SnakeBodyPart");
            var spriteRenderer = bodyPartGO.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _config.SnakeBodySprite;
            spriteRenderer.sortingOrder = _config.BodyPartSortingOrder;
            return bodyPartGO.AddComponent<SnakeBodyPartView>();
        }
    }
}