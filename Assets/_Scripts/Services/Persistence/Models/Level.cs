using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace GameCode.Persistence.Models
{
    [Serializable]
    public class Level : Base
    {
        public int score;
        public int highestScore;
    }
}