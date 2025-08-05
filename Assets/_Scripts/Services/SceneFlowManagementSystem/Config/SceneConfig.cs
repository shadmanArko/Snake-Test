using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Scripts.Services.SceneFlowManagementSystem.Config
{
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "Game/Config/SceneConfig", order = 0)]
    public class SceneConfig : ScriptableObject
    {
        [SerializeField] private AssetReference bootStrapScene;
        [SerializeField] private AssetReference mainMenuScene;
        [SerializeField] private AssetReference loadingScene;
        [SerializeField] private AssetReference gameScene;
        
        public AssetReference BootStrapScene => bootStrapScene;
        public AssetReference MainMenuScene => mainMenuScene;
        public AssetReference LoadingScene => loadingScene;
        public AssetReference GameScene => gameScene;
    }
}