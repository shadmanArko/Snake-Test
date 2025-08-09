using System;
using _Scripts.GlobalConfigs;

namespace _Scripts.Services.RemoteConfig
{
    [Serializable]
    public class GameConfigData
    {
        // Grid
        public int gridWidth = 20;
        public int gridHeight = 20;
        
        // Snake
        public float snakeMoveInterval = 0.2f;
        public string snakeHeadSpriteAddressableKey = "SnakeHead";
        public string snakeBodySpriteAddressableKey = "SnakeBody";
        
        // Food
        public int maxFoodSpawnAttempts = 1000;
        public string foodSpriteAddressableKey = "FoodApple";
        
        // Score
        public int scorePerFood = 10;
        
        // Constructor to create from ScriptableObject
        public GameConfigData(GameConfig config)
        {
            gridWidth = config.gridWidth;
            gridHeight = config.gridHeight;
            snakeMoveInterval = config.snakeMoveInterval;
            snakeHeadSpriteAddressableKey = config.snakeHeadSpriteAddressableKey;
            snakeBodySpriteAddressableKey = config.snakeBodySpriteAddressableKey;
            maxFoodSpawnAttempts = config.maxFoodSpawnAttempts;
            foodSpriteAddressableKey = config.foodSpriteAddressableKey;
            scorePerFood = config.scorePerFood;
        }
        
        // Default constructor
        public GameConfigData() { }
        
        // Method to apply data to ScriptableObject
        public void ApplyToScriptableObject(GameConfig config)
        {
            config.gridWidth = gridWidth;
            config.gridHeight = gridHeight;
            config.snakeMoveInterval = snakeMoveInterval;
            config.snakeHeadSpriteAddressableKey = snakeHeadSpriteAddressableKey;
            config.snakeBodySpriteAddressableKey = snakeBodySpriteAddressableKey;
            config.maxFoodSpawnAttempts = maxFoodSpawnAttempts;
            config.foodSpriteAddressableKey = foodSpriteAddressableKey;
            config.scorePerFood = scorePerFood;
        }
    }
}