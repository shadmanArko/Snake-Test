using System;
using _Scripts.Entities.MainMenu.Controller;
using _Scripts.Entities.MainMenu.Model;
using _Scripts.Entities.MainMenu.View;
using UniRx;
using UnityEngine;
using Zenject;

public class MainMenuGameSceneInit : IInitializable, IDisposable
{
    [Inject] private CompositeDisposable _disposables;
    public void Dispose()
    {
        _disposables?.Dispose();
    }

    public void Initialize()
    {
    }
}

[CreateAssetMenu(fileName = "MainMenuSceneInstaller", menuName = "Installers/MainMenuSceneInstaller")]
public class MainMenuSceneInstaller : ScriptableObjectInstaller<MainMenuSceneInstaller>
{
    [SerializeField] private MainMenuView mainMenuView;
    public override void InstallBindings()
    {
        Container.Bind<CompositeDisposable>().AsSingle();
        Container.Bind<MainMenuGameSceneInit>().AsSingle().NonLazy();
        
        Container.BindInterfacesTo<MainMenuModel>().AsSingle();
        Container.BindInterfacesTo<MainMenuView>().FromComponentInNewPrefab(mainMenuView).AsSingle();
        Container.BindInterfacesTo<MainMenuController>().AsSingle();
    }
}