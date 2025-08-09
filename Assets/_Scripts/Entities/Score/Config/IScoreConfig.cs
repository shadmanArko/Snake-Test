using _Scripts.GlobalConfigs;

namespace _Scripts.Entities.Score.Config
{
    public interface IScoreConfig
    {
        GameLevelConfig GameLevelConfig { get; set; }
    }
}