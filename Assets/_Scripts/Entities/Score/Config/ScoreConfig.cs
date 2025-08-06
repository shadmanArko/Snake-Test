using UnityEngine;

namespace _Scripts.Entities.Score.Config
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Game/Config/ScoreConfig", order = 0)]
    public class ScoreConfig : ScriptableObject, IScoreConfig
    {
        [SerializeField] private string levelId = "level01";
        
        [Header("Score")]
        [SerializeField] private int scorePerFood = 10;
        
        public string LevelId
        {
            get => levelId;
            set => levelId = value;
        }

        public int ScorePerFood
        {
            get => scorePerFood;
            set => scorePerFood = value;
        }
    }
}