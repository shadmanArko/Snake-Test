using System;
using System.Collections.Generic;
using GameCode.Persistence.Models;

namespace GameCode.Persistence
{
    [Serializable]
    public class SaveData
    {
        public List<Level> levels;
    }
}