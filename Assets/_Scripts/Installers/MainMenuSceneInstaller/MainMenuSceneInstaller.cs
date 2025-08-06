using _Scripts.Entities.MainMenu.Controller;
using _Scripts.Entities.MainMenu.Model;
using _Scripts.Entities.MainMenu.View;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MainMenuSceneInstaller", menuName = "Installers/MainMenuSceneInstaller")]
public class MainMenuSceneInstaller : ScriptableObjectInstaller<MainMenuSceneInstaller>
{
    [SerializeField] private MainMenuView mainMenuView;
    public override void InstallBindings()
    {
        Container.Bind<IMainMenuModel>().To<MainMenuModel>().AsSingle().NonLazy();
        Container.Bind<MainMenuView>().FromComponentInNewPrefab(mainMenuView).AsSingle().NonLazy();
        Container.Bind<IMainMenuController>().To<MainMenuController>().AsSingle().NonLazy();
    }
}