using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Food.Config
{
    [CreateAssetMenu(fileName = "FoodConfig", menuName = "Game/Config/FoodConfig")]
    public class FoodConfig : ScriptableObject, IFoodConfig
    {
        [Header("Grid")] 
        [SerializeField] private GameConfig gameConfig;
        
        public GameConfig GameConfig
        {
            get => gameConfig;
            set => gameConfig = value;
        }
        
    }
}