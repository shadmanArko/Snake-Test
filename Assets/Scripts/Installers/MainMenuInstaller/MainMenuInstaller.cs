using MainMenu.Controller;
using MainMenu.Model;
using MainMenu.View;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MainMenuInstaller", menuName = "Installers/MainMenuInstaller")]
public class MainMenuInstaller : ScriptableObjectInstaller<MainMenuInstaller>
{
    [SerializeField] private MainMenuView mainMenuView;
    public override void InstallBindings()
    {
        // Model
        Container.Bind<IMainMenuModel>().To<MainMenuModel>().AsSingle().NonLazy();
        
        // View
        Container.Bind<IMainMenuView>().FromComponentInNewPrefab(mainMenuView).AsSingle().NonLazy();
        
        // Controller
        Container.Bind<IMainMenuController>().To<MainMenuController>().AsSingle().NonLazy();
    }
}