using UnityEngine;

namespace _Scripts.Entities.GameOverAndPause.Config
{
    [CreateAssetMenu(fileName = "GameOverAndPauseConfig", menuName = "Game/Config/GameOverAndPause/GameOverAndPauseConfig", order = 0)]
    public class GameOverAndPauseConfig : ScriptableObject
    {
        [SerializeField] private GameStateConfig gameOverstateConfig;
        [SerializeField] private GameStateConfig pauseStateConfig;
        [SerializeField] private GameStateConfig gameResumeConfig;
        
        public GameStateConfig GameOverStateConfig => gameOverstateConfig;
        public GameStateConfig PauseStateConfig => pauseStateConfig;
        public GameStateConfig GameResumeConfig => gameResumeConfig;
    }
}