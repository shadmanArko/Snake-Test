using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Food.Config
{
    [CreateAssetMenu(fileName = "FoodConfig", menuName = "Game/Config/FoodConfig")]
    public class FoodConfig : ScriptableObject, IFoodConfig
    {
        [Header("Grid")] 
        [SerializeField] private GridConfig _gridConfig;
    
        [Header("Food")]
        [SerializeField] private string _foodSpriteAddressableKey;
        
       
        public GridConfig GridConfig
        {
            get => _gridConfig;
            set => _gridConfig = value;
        }

        public string FoodSpriteAddressableKey
        {
            get => _foodSpriteAddressableKey;
            set => _foodSpriteAddressableKey = value;
        }
    }
}