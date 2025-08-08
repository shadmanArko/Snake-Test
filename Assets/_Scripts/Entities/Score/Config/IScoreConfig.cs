using _Scripts.GlobalConfigs;

namespace _Scripts.Entities.Score.Config
{
    public interface IScoreConfig
    {
        GameConfig GameConfig { get; set; }
        GameLevelConfig GameLevelConfig { get; set; }
    }
}