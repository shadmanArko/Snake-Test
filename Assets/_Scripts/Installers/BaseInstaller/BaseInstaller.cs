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
        Container.Bind<Levels>().AsSingle();
        Container.Bind<TextAsset>().FromInstance(_saveDataJsonFile).AsSingle();
        Container.Bind<DataContext>().To<JsonDataContext>().AsSingle();
        Container.BindInterfacesTo<PersistenceSystemInitializer>().AsSingle();
        Container.BindInterfacesTo<UnitOfWork>().AsSingle();
        
        Container.BindInterfacesTo<UniRxEventBus>().AsSingle();
        
        Container.BindInterfacesTo<SoundConfig>().FromScriptableObject(_soundConfig).AsSingle();
        Container.BindInterfacesTo<SoundManager>().AsSingle();

        Container.BindInterfacesTo<SceneFlowManager>().AsSingle();
        Container.BindInterfacesTo<SceneConfig>().FromScriptableObject(_sceneConfig).AsSingle();
    }
}