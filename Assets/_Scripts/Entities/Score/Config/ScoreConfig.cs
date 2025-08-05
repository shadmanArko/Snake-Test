using UnityEngine;

namespace _Scripts.Entities.Score.Config
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Game/Config/ScoreConfig", order = 0)]
    public class ScoreConfig : ScriptableObject
    {
        public string levelId = "level01";
        
        [Header("Score")]
        public int scorePerFood = 10;
    }
}