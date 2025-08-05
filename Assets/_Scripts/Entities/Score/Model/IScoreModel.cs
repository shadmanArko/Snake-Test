using UniRx;

namespace ScoreSystem.Model
{
    public interface IScoreModel
    {
        public IReadOnlyReactiveProperty<int> Score { get; }
        public IReadOnlyReactiveProperty<int> HighScore { get; }
        void UpdateScore();
    }
}