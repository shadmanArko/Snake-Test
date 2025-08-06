using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Entities.Score.View
{
    public interface IScoreView
    {
        Text ScoreText { get; }
        Text HighScoreText { get; }
    }

    public class ScoreView : MonoBehaviour, IScoreView
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _highScoreText;
        
        public Text ScoreText => _scoreText;
        public Text HighScoreText => _highScoreText;
    }
}