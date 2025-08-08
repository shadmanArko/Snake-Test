using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.GlobalConfigs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [Header("Grid")]
        public int gridWidth = 20;
        public int gridHeight = 20;
        
        [Header("Snake")]
        public float snakeMoveInterval = 0.2f;
        public string snakeHeadSpriteAddressableKey = "SnakeHead";
        public string snakeBodySpriteAddressableKey = "SnakeBody";
        
        [Header("Food")] 
        public int maxFoodSpawnAttempts = 1000;
        public string foodSpriteAddressableKey = "FoodApple";
        
        [Header("Score")]
        public int scorePerFood = 10;
    }
}