using UniRx;

namespace _Scripts.Entities.Score.Model
{
    public interface IScoreModel
    {
        public IReadOnlyReactiveProperty<int> Score { get; }
        public IReadOnlyReactiveProperty<int> HighScore { get; }
        void UpdateScore();
    }
}