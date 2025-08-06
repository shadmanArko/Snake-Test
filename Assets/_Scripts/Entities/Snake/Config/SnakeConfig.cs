using _Scripts.Enums;
using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Snake.Config
{
    [CreateAssetMenu(fileName = "SnakeConfig", menuName = "Game/Config/SnakeConfig")]
    public class SnakeConfig : ScriptableObject, ISnakeConfig
    {
        [Header("Movement")] 
        [SerializeField] private float _moveInterval = 0.2f;
        [SerializeField] private Vector2Int _startPosition = new Vector2Int(10, 10);
        [SerializeField] private Direction _startDirection = Direction.Right;

        [Header("Body")] 
        [SerializeField] private Sprite _snakeBodySprite;
        [SerializeField] private int _bodyPartSortingOrder = 1;

        [Header("Sounds")] 
        [SerializeField] private bool _enableSounds = true;

        [Header("Grid")] 
        [SerializeField] private GridConfig _gridConfig;
        
        public float MoveInterval
        {
            get => _moveInterval;
            set => _moveInterval = value;
        }

        public Vector2Int StartPosition
        {
            get => _startPosition;
            set => _startPosition = value;
        }

        public Direction StartDirection
        {
            get => _startDirection;
            set => _startDirection = value;
        }

        public Sprite SnakeBodySprite
        {
            get => _snakeBodySprite;
            set => _snakeBodySprite = value;
        }

        public int BodyPartSortingOrder
        {
            get => _bodyPartSortingOrder;
            set => _bodyPartSortingOrder = value;
        }

        public bool EnableSounds
        {
            get => _enableSounds;
            set => _enableSounds = value;
        }

        public GridConfig GridConfig
        {
            get => _gridConfig;
            set => _gridConfig = value;
        }
    }
}