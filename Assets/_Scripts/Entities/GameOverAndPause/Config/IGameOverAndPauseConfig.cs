using _Scripts.GlobalConfigs;

namespace _Scripts.Entities.GameOverAndPause.Config
{
    public interface IGameOverAndPauseConfig
    {
        GameStateConfig GameOverStateConfig { get; }
        GameStateConfig PauseStateConfig { get; }
        GameStateConfig GameResumeConfig { get; }
        GameLevelConfig GameLevelConfig { get; }
    }
}