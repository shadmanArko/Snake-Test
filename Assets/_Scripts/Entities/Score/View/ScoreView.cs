using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Entities.Score.View
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _highScoreText;
        
        public Text ScoreText => _scoreText;
        public Text HighScoreText => _highScoreText;
    }
}