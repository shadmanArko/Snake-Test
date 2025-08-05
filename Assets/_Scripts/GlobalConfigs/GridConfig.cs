using UnityEngine;

namespace _Scripts.GlobalConfigs
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Game/Config/GridConfig", order = 0)]
    public class GridConfig : ScriptableObject
    {
        [Header("Grid")]
        public int width = 20;
        public int height = 20;
    }
}