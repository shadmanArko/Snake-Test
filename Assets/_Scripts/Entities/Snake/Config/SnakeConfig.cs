using _Scripts.Enums;
using GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Snake.Config
{
    [CreateAssetMenu(fileName = "SnakeConfig", menuName = "Game/Config/SnakeConfig")]
    public class SnakeConfig : ScriptableObject
    {
        [Header("Movement")] public float moveInterval = 0.2f;
        public Vector2Int startPosition = new Vector2Int(10, 10);
        public Direction startDirection = Direction.Right;

        [Header("Body")] public Sprite snakeBodySprite;
        public int bodyPartSortingOrder = 1;

        [Header("Sounds")] 
        public bool enableSounds = true;

        [Header("Grid")] 
        public GridConfig gridConfig;
    }
}