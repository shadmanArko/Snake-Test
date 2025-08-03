using _Scripts.EventBus;
using GameCode.Persistence;
using GameCode.Persistence.Repositories;
using SceneFlowManagementSystem.Config;
using SceneFlowManagementSystem.SceneFlowManager;
using SceneLoaderSystem;
using SoundSystem.Config;
using SoundSystem.SoundManager;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GlobalInstaller", menuName = "Installers/GlobalInstaller")]
public class GlobalInstaller : ScriptableObjectInstaller<GlobalInstaller>
{
    [SerializeField] private SoundConfig _soundConfig;
    
    [SerializeField] private TextAsset _saveDataJsonFile;
    
    [SerializeField] private SceneConfig _sceneConfig;
    public override void InstallBindings()
    {
        Container.Bind<TextAsset>().FromInstance(_saveDataJsonFile).AsSingle();
        Container.Bind<DataContext>().To<JsonDataContext>().AsSingle();
        Container.Bind<Initializer>().AsSingle().NonLazy();
        Container.Bind<Levels>().AsSingle();
        Container.Bind<UnitOfWork>().AsSingle().NonLazy();
        
        Container.Bind<IEventBus>().To<UniRxEventBus>().AsSingle();
        Container.Bind<SoundConfig>().FromScriptableObject(_soundConfig).AsSingle();
        Container.Bind<SceneConfig>().FromScriptableObject(_sceneConfig).AsSingle();
        Container.Bind<ISoundManager>().To<SoundSystem.SoundManager.SoundManager>().AsSingle().NonLazy();
        Container.Bind<SceneLoader>().AsSingle();
        Container.Bind<SceneFlowManager>().AsSingle().NonLazy();
    }
}