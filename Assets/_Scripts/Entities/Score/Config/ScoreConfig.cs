using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.Score.Config
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Game/Config/ScoreConfig", order = 0)]
    public class ScoreConfig : ScriptableObject, IScoreConfig
    {
        [SerializeField] private GameLevelConfig gameLevelConfig;
        [SerializeField] private GameConfig gameConfig;
        public GameLevelConfig GameLevelConfig
        {
            get => gameLevelConfig;
            set => gameLevelConfig = value;
        }
        public GameConfig GameConfig
        {
            get => gameConfig;
            set => gameConfig = value;
        }
    }
}