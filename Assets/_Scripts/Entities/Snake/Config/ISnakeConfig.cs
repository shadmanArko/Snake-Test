using _Scripts.Enums;
using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Snake.Config
{
    public interface ISnakeConfig
    {
        float MoveInterval { get; set; }
        Vector2Int StartPosition { get; set; }
        Direction StartDirection { get; set; }
        Sprite SnakeBodySprite { get; set; }
        int BodyPartSortingOrder { get; set; }
        bool EnableSounds { get; set; }
        GridConfig GridConfig { get; set; }
    }
}