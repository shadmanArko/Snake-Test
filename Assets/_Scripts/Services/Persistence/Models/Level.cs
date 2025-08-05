using System;

namespace _Scripts.Services.Persistence.Models
{
    [Serializable]
    public class Level : Base
    {
        public int score;
        public int highestScore;
    }
}