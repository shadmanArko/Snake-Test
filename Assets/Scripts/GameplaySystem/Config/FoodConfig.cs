using GlobalConfigs;
using UnityEngine;

namespace LevelSystem.Config
{
    [CreateAssetMenu(fileName = "FoodConfig", menuName = "Game/Config/FoodConfig")]
    public class FoodConfig : ScriptableObject
    {
        [Header("Grid")] 
        public GridConfig gridConfig;
    
        [Header("Food")]
        public Sprite foodSprite;
    }
}