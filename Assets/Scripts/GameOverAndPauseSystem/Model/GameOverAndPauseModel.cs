using _Scripts.EventBus;
using GameOverAndPauseSystem.Config;
using UnityEngine;

namespace GameOverAndPauseSystem.Model
{
    public class GameOverAndPauseModel : IGameOverAndPauseModel
    {
        private readonly GameOverAndPauseConfig _config;

        public GameOverAndPauseModel(GameOverAndPauseConfig config)
        {
            _config = config;
        }

        public GameStateConfig GameOver()
        {
            return _config.GameOverStateConfig;
        }
        

        public void Pause()
        {
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            Time.timeScale = 1f;
        }
    }
}