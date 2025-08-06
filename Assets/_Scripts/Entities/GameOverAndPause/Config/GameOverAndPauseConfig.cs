using _Scripts.GlobalConfigs;
using UnityEngine;

namespace _Scripts.Entities.GameOverAndPause.Config
{
    [CreateAssetMenu(fileName = "GameOverAndPauseConfig", menuName = "Game/Config/GameOverAndPause/GameOverAndPauseConfig", order = 0)]
    public class GameOverAndPauseConfig : ScriptableObject, IGameOverAndPauseConfig
    {
        [SerializeField] private GameStateConfig gameOverstateConfig;
        [SerializeField] private GameStateConfig pauseStateConfig;
        [SerializeField] private GameStateConfig gameResumeConfig;
        [SerializeField] private GameLevelConfig gameLevelConfig;
        
        public GameStateConfig GameOverStateConfig => gameOverstateConfig;
        public GameStateConfig PauseStateConfig => pauseStateConfig;
        public GameStateConfig GameResumeConfig => gameResumeConfig;
        public GameLevelConfig GameLevelConfig => gameLevelConfig;
    }
}