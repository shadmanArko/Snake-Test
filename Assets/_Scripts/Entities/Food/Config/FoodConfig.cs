using GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Food.Config
{
    [CreateAssetMenu(fileName = "FoodConfig", menuName = "Game/Config/FoodConfig")]
    public class FoodConfig : ScriptableObject
    {
        [Header("Grid")] 
        public GridConfig gridConfig;
    
        [Header("Food")]
        public string foodSpriteAddressableKey;
    }
}