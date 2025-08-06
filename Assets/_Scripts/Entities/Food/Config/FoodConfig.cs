using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Food.Config
{
    public interface IFoodConfig
    {
    }

    [CreateAssetMenu(fileName = "FoodConfig", menuName = "Game/Config/FoodConfig")]
    public class FoodConfig : ScriptableObject, IFoodConfig
    {
        [Header("Grid")] 
        public GridConfig gridConfig;
    
        [Header("Food")]
        public string foodSpriteAddressableKey;
    }
}