using UnityEngine.AddressableAssets;

namespace _Scripts.Services.SceneFlowManagementSystem.Config
{
    public interface ISceneConfig
    {
        AssetReference BootStrapScene { get; }
        AssetReference MainMenuScene { get; }
        AssetReference LoadingScene { get; }
        AssetReference GameScene { get; }
    }
}