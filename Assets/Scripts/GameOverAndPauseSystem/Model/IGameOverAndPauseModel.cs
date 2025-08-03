using GameOverAndPauseSystem.Config;

namespace GameOverAndPauseSystem.Model
{
    public interface IGameOverAndPauseModel
    {
        GameStateConfig GameOver();
        void Pause();
        void Resume();
    }
}