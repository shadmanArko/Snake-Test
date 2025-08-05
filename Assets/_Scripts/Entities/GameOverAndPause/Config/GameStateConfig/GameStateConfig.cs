using UnityEngine;

namespace GameOverAndPauseSystem.Config
{
    [CreateAssetMenu(fileName = "GameStateConfig", menuName = "Game/Config/GameOverAndPause/GameStateConfig", order = 0)]
    public class GameStateConfig : ScriptableObject
    {
        [SerializeField] private bool showPauseUI;
        [SerializeField] private bool showGameOverUI;
        
        public bool ShowPauseUI => showPauseUI;
        public bool ShowGameOverUI => showGameOverUI;
    }
}