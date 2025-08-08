using _Scripts.Enums;
using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Snake.Config
{
    public interface ISnakeConfig
    {
        Vector2Int StartPosition { get; set; }
        Direction StartDirection { get; set; }
        int BodyPartSortingOrder { get; set; }
        bool EnableSounds { get; set; }
    }
}