using UnityEngine;

namespace LevelSystem.Config
{
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "Game/Config/GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [Header("Grid")]
        public int width = 20;
        public int height = 20;
    
        [Header("Food")]
        public Sprite foodSprite;
        public int foodSortingOrder = 2;
        
    }
}