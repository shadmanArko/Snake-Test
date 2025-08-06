using UnityEngine;

namespace _Scripts.GlobalConfigs
{
    [CreateAssetMenu(fileName = "GameLevelConfig", menuName = "Game/Config/GameLevelConfig", order = 0)]
    public class GameLevelConfig : ScriptableObject
    {
        [SerializeField] private string _levelId;
        
        public string LevelId => _levelId;
    }
}