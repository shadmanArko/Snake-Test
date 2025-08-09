using _Scripts.Entities.GameOverAndPause.Config;

namespace _Scripts.Entities.GameOverAndPause.Model
{
    public interface IGameOverAndPauseModel
    {
        GameStateConfig GameOver();
        GameStateConfig Pause();
        GameStateConfig Resume();
        void QuitGame();
        bool ShowNewHighScoreTextTitle();
        int GetScore();
        int GetHighScore();
    }
}