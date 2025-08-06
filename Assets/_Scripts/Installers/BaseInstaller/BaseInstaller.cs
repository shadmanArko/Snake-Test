using _Scripts.Services.EventBus.Core;
using _Scripts.Services.Persistence;
using _Scripts.Services.Persistence.Repositories;
using _Scripts.Services.SceneFlowManagementSystem.Config;
using _Scripts.Services.SceneFlowManagementSystem.SceneFlowManager;
using _Scripts.Services.SoundSystem.Config;
using _Scripts.Services.SoundSystem.SoundManager;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "BaseInstaller", menuName = "Installers/BaseInstaller")]
public class BaseInstaller : ScriptableObjectInstaller<BaseInstaller>
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
        Container.Bind<ISoundManager>().To<_Scripts.Services.SoundSystem.SoundManager.SoundManager>().AsSingle().NonLazy();
        Container.Bind<SceneFlowManager>().AsSingle().NonLazy();
    }
}