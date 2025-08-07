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
        
        [Header("AssetKey")] 
        [SerializeField] private string _snakeHeadSpriteAddressableKey;
        [SerializeField] private string _snakeBodySpriteAddressableKey;
        

        [Header("Body")] 
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

        public string SnakeHeadSpriteAddressableKey 
        { 
            get => _snakeHeadSpriteAddressableKey; 
            set => _snakeHeadSpriteAddressableKey = value; 
        }

        public string SnakeBodySpriteAddressableKey
        {
            get => _snakeBodySpriteAddressableKey; 
            set => _snakeBodySpriteAddressableKey = value;
        }
    }
}