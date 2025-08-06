using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Score.Config
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Game/Config/ScoreConfig", order = 0)]
    public class ScoreConfig : ScriptableObject, IScoreConfig
    {
        [SerializeField] private GameLevelConfig gameLevelConfig;
        
        [Header("Score")]
        [SerializeField] private int scorePerFood = 10;
        
        public GameLevelConfig GameLevelConfig
        {
            get => gameLevelConfig;
            set => gameLevelConfig = value;
        }
        
        public int ScorePerFood
        {
            get => scorePerFood;
            set => scorePerFood = value;
        }
    }
}