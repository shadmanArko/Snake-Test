using _Scripts.GlobalConfigs;

namespace _Scripts.Entities.Score.Config
{
    public interface IScoreConfig
    {
        int ScorePerFood { get; set; }
        GameLevelConfig GameLevelConfig { get; set; }
    }
}